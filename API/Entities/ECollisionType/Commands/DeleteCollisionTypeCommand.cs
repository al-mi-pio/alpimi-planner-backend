using AlpimiAPI.Database;
using MediatR;

namespace AlpimiAPI.Entities.ECollisionType.Commands
{
    public record DeleteCollisionTypeCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteCollisionTypeHandler : IRequestHandler<DeleteCollisionTypeCommand>
    {
        private readonly IDbService _dbService;

        public DeleteCollisionTypeHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(
            DeleteCollisionTypeCommand request,
            CancellationToken cancellationToken
        )
        {
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [CollisionType] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE ct
                            FROM [CollisionType] ct
                            INNER JOIN [Schedule] s ON s.[Id] = ct.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND ct.[Id] = @Id;",
                        request
                    );
                    break;
            }
        }
    }
}
