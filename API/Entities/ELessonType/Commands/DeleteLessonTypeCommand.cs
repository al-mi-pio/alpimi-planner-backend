using AlpimiAPI.Database;
using MediatR;

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
