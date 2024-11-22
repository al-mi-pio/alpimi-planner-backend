using AlpimiAPI.Database;
using MediatR;

namespace AlpimiAPI.Entities.ESchedule.Commands
{
    public record DeleteScheduleCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteScheduleHandler : IRequestHandler<DeleteScheduleCommand>
    {
        private readonly IDbService _dbService;

        public DeleteScheduleHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
        {
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete(
                        @"
                            DELETE [Schedule] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE [Schedule] 
                            WHERE [Id] = @Id and [UserId] = @FilteredId;",
                        request
                    );
                    break;
            }
        }
    }
}
