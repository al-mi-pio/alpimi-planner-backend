using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ELesson.Queries;
using AlpimiAPI.Entities.ELessonBlock.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ELessonBlock.Commands
{
    public record DeleteLessonBlockCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteLessonBlockHandler : IRequestHandler<DeleteLessonBlockCommand>
    {
        private readonly IDbService _dbService;

        public DeleteLessonBlockHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(
            DeleteLessonBlockCommand request,
            CancellationToken cancellationToken
        )
        {
            GetLessonBlockHandler getLessonBlockHandler = new GetLessonBlockHandler(_dbService);
            GetLessonBlockQuery getLessonBlockQuery = new GetLessonBlockQuery(
                request.Id,
                new Guid(),
                "Admin"
            );
            ActionResult<LessonBlock?> lessonBlock = await getLessonBlockHandler.Handle(
                getLessonBlockQuery,
                cancellationToken
            );

            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [LessonBlock] 
                            WHERE [Id] = @Id OR [ClusterId] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE l
                            FROM [LessonBlock] lb
                            INNER JOIN [Lesson] l ON l.[Id] = lb.[LessonId]
                            INNER JOIN [LessonType] lt ON lt.[Id] = l.[LessonTypeId]
                            INNER JOIN [Schedule] s ON s.[Id] = lt.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND (lb.[Id] = @Id OR lb.[ClusterId] = @Id);",
                        request
                    );
                    break;
            }

            if (lessonBlock.Value != null)
            {
                await Utilities.CurrentLessonHours.Update(
                    _dbService,
                    lessonBlock.Value.LessonId,
                    cancellationToken
                );
            }
        }
    }
}
