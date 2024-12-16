using AlpimiAPI.Database;
using MediatR;

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
            var lessonId = await _dbService.Get<Guid?>(
                $@"
                    SELECT DISTINCT
                    [LessonId] 
                    FROM [LessonBlock] 
                    WHERE [Id] = '{request.Id}' OR [ClusterId] = '{request.Id}';",
                ""
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
            if (lessonId != null)
            {
                await Utilities.CurrentLessonHours.Update(
                    _dbService,
                    lessonId.Value,
                    cancellationToken
                );
            }
        }
    }
}
