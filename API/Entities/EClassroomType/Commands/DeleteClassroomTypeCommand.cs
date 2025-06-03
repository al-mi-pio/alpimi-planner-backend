using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroomType.DTO;
using AlpimiAPI.Entities.EClassroomType.Queries;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.EClassroomType.Commands
{
    public record DeleteClassroomTypeCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteClassroomTypeHandler : IRequestHandler<DeleteClassroomTypeCommand>
    {
        private readonly IDbService _dbService;

        public DeleteClassroomTypeHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(
            DeleteClassroomTypeCommand request,
            CancellationToken cancellationToken
        )
        {
            GetClassroomTypeHandler getClassroomTypeHandler = new GetClassroomTypeHandler(
                _dbService
            );
            GetClassroomTypeQuery getClassroomTypeQuery = new GetClassroomTypeQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<ClassroomType?> classroomtype = await getClassroomTypeHandler.Handle(
                getClassroomTypeQuery,
                cancellationToken
            );
            if (classroomtype.Value != null)
            {
                CreateClassroomTypeDTO reversaleDTO = new CreateClassroomTypeDTO
                {
                    Name = classroomtype.Value.Name,
                    ScheduleId = classroomtype.Value.ScheduleId,
                };
                AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
                AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    AffectedEntityId = request.Id,
                    AffectedEntity = "ClassroomType",
                    Command = "Delete",
                    ReversaleDTO = JsonSerializer.Serialize(reversaleDTO),
                    CollisionChecked = true,
                    ScheduleId = reversaleDTO.ScheduleId,
                };
                AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
                await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);
            }
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [LessonClassroomType] 
                            WHERE [ClassroomTypeId] = @Id;",
                        request
                    );
                    await _dbService.Delete(
                        @"
                            DELETE [ClassroomClassroomType] 
                            WHERE [ClassroomTypeId] = @Id;",
                        request
                    );
                    await _dbService.Delete(
                        @"
                            DELETE [ClassroomType] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE cct
                            FROM [ClassroomClassroomType] cct
                            INNER JOIN [ClassroomType] ct ON ct.[Id] = cct.[ClassroomTypeId]
                            INNER JOIN [Schedule] s ON s.[Id] = ct.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND cct.[ClassroomTypeId] = @Id;",
                        request
                    );
                    await _dbService.Delete(
                        @"
                            DELETE lct
                            FROM [LessonClassroomType] lct
                            INNER JOIN [ClassroomType] ct ON ct.[Id] = lct.[ClassroomTypeId]
                            INNER JOIN [Schedule] s ON s.[Id] = ct.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND lct.[ClassroomTypeId] = @Id;",
                        request
                    );
                    await _dbService.Delete(
                        @"
                            DELETE ct
                            FROM [ClassroomType] ct
                            INNER JOIN [Schedule] s ON s.[Id] = ct.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND ct.[Id] = @Id;",
                        request
                    );
                    break;
            }
        }
    }
}
