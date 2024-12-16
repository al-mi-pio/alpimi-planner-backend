using AlpimiAPI.Database;
using MediatR;

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
