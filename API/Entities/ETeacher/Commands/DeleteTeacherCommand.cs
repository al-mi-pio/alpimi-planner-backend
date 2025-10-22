using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ETeacher.DTO;
using AlpimiAPI.Entities.ETeacher.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ETeacher.Commands
{
    public record DeleteTeacherCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteTeacherHandler : IRequestHandler<DeleteTeacherCommand>
    {
        private readonly IDbService _dbService;

        public DeleteTeacherHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(DeleteTeacherCommand request, CancellationToken cancellationToken)
        {
            GetTeacherHandler getTeacherHandler = new GetTeacherHandler(_dbService);
            GetTeacherQuery getTeacherQuery = new GetTeacherQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<Teacher?> teacher = await getTeacherHandler.Handle(
                getTeacherQuery,
                cancellationToken
            );
            if (teacher.Value != null)
            {
                CreateTeacherDTO reversaleDTO = new CreateTeacherDTO
                {
                    Name = teacher.Value.Name,
                    Surname = teacher.Value.Surname,
                    Email = teacher.Value.Email,
                    ScheduleId = teacher.Value.ScheduleId,
                };
                AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
                AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    AffectedEntityId = request.Id,
                    AffectedEntity = "Teacher",
                    Command = "Delete",
                    ReversaleDTO = JsonSerializer.Serialize(reversaleDTO),
                    CollisionChecked = false,
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
                            DELETE FROM [Lesson] 
                            WHERE [TeacherId] = @Id;",
                        request
                    );
                    await _dbService.Delete(
                        @"
                            DELETE [Teacher] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE l
                            FROM [Lesson] l
                            INNER JOIN [LessonType] lt ON lt.[Id] = l.[LessonTypeId]
                            INNER JOIN [Schedule] s ON s.[Id] = lt.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND l.[TeacherId] = @Id;",
                        request
                    );
                    await _dbService.Delete(
                        @"
                            DELETE t
                            FROM [Teacher] t
                            INNER JOIN [Schedule] s ON s.[Id] = t.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND t.[Id] = @Id;",
                        request
                    );
                    break;
            }
        }
    }
}
