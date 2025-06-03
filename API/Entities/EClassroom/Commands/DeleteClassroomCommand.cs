using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom.DTO;
using AlpimiAPI.Entities.EClassroom.Queries;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.EClassroom.Commands
{
    public record DeleteClassroomCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteClassroomHandler : IRequestHandler<DeleteClassroomCommand>
    {
        private readonly IDbService _dbService;

        public DeleteClassroomHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(
            DeleteClassroomCommand request,
            CancellationToken cancellationToken
        )
        {
            await _dbService.Raw(
                "ALTER TABLE LessonBlock NOCHECK CONSTRAINT FK_LessonBlock_Classroom_ClassroomId"
            );
            GetClassroomHandler getClassroomHandler = new GetClassroomHandler(_dbService);
            GetClassroomQuery getClassroomQuery = new GetClassroomQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<Classroom?> classroom = await getClassroomHandler.Handle(
                getClassroomQuery,
                cancellationToken
            );
            if (classroom.Value != null)
            {
                CreateClassroomDTO reversaleDTOClassroom = new CreateClassroomDTO
                {
                    Capacity = classroom.Value.Capacity,
                    Name = classroom.Value.Name,
                    ScheduleId = classroom.Value.ScheduleId,
                };
                AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
                AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    AffectedEntityId = request.Id,
                    AffectedEntity = "Classroom",
                    Command = "Delete",
                    ReversaleDTO = JsonSerializer.Serialize(reversaleDTOClassroom),
                    CollisionChecked = true,
                    ScheduleId = reversaleDTOClassroom.ScheduleId,
                };
                AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
                await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);
            }
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [Classroom] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE c
                            FROM [Classroom] c
                            INNER JOIN [Schedule] s ON s.[Id] = c.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND c.[Id] = @Id;",
                        request
                    );
                    break;
            }
            await _dbService.Raw(
                "ALTER TABLE LessonBlock CHECK CONSTRAINT FK_LessonBlock_Classroom_ClassroomId"
            );
        }
    }
}
