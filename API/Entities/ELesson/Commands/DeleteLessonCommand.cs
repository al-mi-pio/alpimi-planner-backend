using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ELesson.DTO;
using AlpimiAPI.Entities.ELesson.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ELesson.Commands
{
    public record DeleteLessonCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteLessonHandler : IRequestHandler<DeleteLessonCommand>
    {
        private readonly IDbService _dbService;

        public DeleteLessonHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(DeleteLessonCommand request, CancellationToken cancellationToken)
        {
            GetLessonHandler getLessonHandler = new GetLessonHandler(_dbService);
            GetLessonQuery getLessonQuery = new GetLessonQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<Lesson?> lesson = await getLessonHandler.Handle(
                getLessonQuery,
                cancellationToken
            );
            if (lesson.Value != null)
            {
                CreateLessonDTO reversaleDTO = new CreateLessonDTO
                {
                    Name = lesson.Value.Name,
                    AmountOfHours = lesson.Value.AmountOfHours,
                    LessonTypeId = lesson.Value.LessonTypeId,
                    TeacherId = lesson.Value.TeacherId,
                    SubgroupIds = []
                };
                AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
                AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    AffectedEntityId = request.Id,
                    AffectedEntity = "Lesson",
                    Command = "Delete",
                    ReversaleDTO = JsonSerializer.Serialize(reversaleDTO),
                    CollisionChecked = true,
                    ScheduleId = lesson.Value.LessonType.ScheduleId,
                };
                AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
                await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);
            }
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [Lesson] 
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
                            WHERE s.[UserId] = @FilteredId AND l.[Id] = @Id;",
                        request
                    );
                    break;
            }
        }
    }
}
