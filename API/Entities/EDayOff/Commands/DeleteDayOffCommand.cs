using AlpimiAPI.Database;
using MediatR;

namespace AlpimiAPI.Entities.EDayOff.Commands
{
    public record DeleteDayOffCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteDayOffHandler : IRequestHandler<DeleteDayOffCommand>
    {
        private readonly IDbService _dbService;

        public DeleteDayOffHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(DeleteDayOffCommand request, CancellationToken cancellationToken)
        {
            switch (request.Role)
            {
                case "Admin":
                    await _dbService.Delete("DELETE [DayOff] WHERE [Id] = @Id;", request);
                    break;
                default:
                    await _dbService.Delete(
                        @"
                            DELETE do
                            FROM [DayOff] do
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = do.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id] = ss.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND do.[Id] = @Id;",
                        request
                    );
                    break;
            }
        }
    }
}
