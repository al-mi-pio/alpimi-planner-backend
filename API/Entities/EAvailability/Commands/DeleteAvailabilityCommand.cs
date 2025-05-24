using AlpimiAPI.Database;
using MediatR;

namespace AlpimiAPI.Entities.EAvailability.Commands
{
    public record DeleteAvailabilityCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteAvailabilityHandler : IRequestHandler<DeleteAvailabilityCommand>
    {
        private readonly IDbService _dbService;

        public DeleteAvailabilityHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(
            DeleteAvailabilityCommand request,
            CancellationToken cancellationToken
        )
        {
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [Availability] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE a
                            FROM [Availability] a
                            INNER JOIN [Teacher] t ON t.[Id] = a.[TeacherId]
                            INNER JOIN [Schedule] s ON s.[Id] = t.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND a.[Id] = @Id;",
                        request
                    );
                    break;
            }
        }
    }
}
