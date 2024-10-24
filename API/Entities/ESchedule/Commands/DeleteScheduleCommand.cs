using MediatR;

namespace AlpimiAPI.Entities.ESchedule.Commands
{
    public record DeleteScheduleCommand(Guid Id) : IRequest;

    public class DeleteScheduleHandler : IRequestHandler<DeleteScheduleCommand>
    {
        private readonly IDbService _dbService;

        public DeleteScheduleHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
        {
            await _dbService.Delete("DELETE [Schedule] WHERE [Id] = @Id;", request);
        }
    }
}
