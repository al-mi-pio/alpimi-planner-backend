using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ELessonType.DTO;
using AlpimiAPI.Entities.ELessonType.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ELessonType.Commands
{
    public record DeleteLessonTypeCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteLessonTypeHandler : IRequestHandler<DeleteLessonTypeCommand>
    {
        private readonly IDbService _dbService;

        public DeleteLessonTypeHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(
            DeleteLessonTypeCommand request,
            CancellationToken cancellationToken
        )
        {
            GetLessonTypeHandler getLessonTypeHandler = new GetLessonTypeHandler(_dbService);
            GetLessonTypeQuery getLessonTypeQuery = new GetLessonTypeQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<LessonType?> lessontype = await getLessonTypeHandler.Handle(
                getLessonTypeQuery,
                cancellationToken
            );
            if (lessontype.Value != null)
            {
                CreateLessonTypeDTO reversaleDTO = new CreateLessonTypeDTO
                {
                    Name = lessontype.Value.Name,
                    Color = lessontype.Value.Color,
                    ScheduleId = lessontype.Value.ScheduleId,
                };
                AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
                AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    AffectedEntityId = request.Id,
                    AffectedEntity = "LessonType",
                    Command = "Delete",
                    ReversaleDTO = JsonSerializer.Serialize(reversaleDTO),
                    CollisionChecked = true,
                    ScheduleId = reversaleDTO.ScheduleId!.Value,
                };
                AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
                await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);
            }
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [LessonType] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE lt
                            FROM [LessonType] lt
                            INNER JOIN [Schedule] s ON s.[Id] = lt.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND lt.[Id] = @Id;",
                        request
                    );
                    break;
            }
        }
    }
}
