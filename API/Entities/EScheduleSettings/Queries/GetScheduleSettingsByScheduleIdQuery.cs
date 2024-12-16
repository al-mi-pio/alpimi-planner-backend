using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.EScheduleSettings.Queries
{
    public record GetScheduleSettingsByScheduleIdQuery(
        Guid ScheduleId,
        Guid FilteredId,
        string Role
    ) : IRequest<ScheduleSettings?>;

    public class GetScheduleSettingsByScheduleIdHandler
        : IRequestHandler<GetScheduleSettingsByScheduleIdQuery, ScheduleSettings?>
    {
        private readonly IDbService _dbService;

        public GetScheduleSettingsByScheduleIdHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<ScheduleSettings?> Handle(
            GetScheduleSettingsByScheduleIdQuery request,
            CancellationToken cancellationToken
        )
        {
            ScheduleSettings? scheduleSettings;
            switch (request.Role)
            {
                case "Admin":
                    scheduleSettings = await _dbService.Get<ScheduleSettings?>(
                        @"
                            SELECT
                            [Id], [SchoolHour], [SchoolYearStart], [SchoolYearEnd], [ScheduleId], [SchoolDays]
                            FROM [ScheduleSettings] 
                            WHERE [ScheduleId] = @ScheduleId;",
                        request
                    );
                    break;
                default:
                    scheduleSettings = await _dbService.Get<ScheduleSettings?>(
                        @"
                            SELECT 
                            ss.[Id], [SchoolHour], [SchoolYearStart], [SchoolYearEnd], [ScheduleId], [SchoolDays]
                            FROM [ScheduleSettings] ss
                            JOIN [Schedule] s ON s.[Id] = ss.[ScheduleId]
                            WHERE [ScheduleId] = @ScheduleId AND s.[UserId] = @FilteredId;",
                        request
                    );
                    break;
            }

            if (scheduleSettings != null)
            {
                GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
                GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                    scheduleSettings.ScheduleId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<Schedule?> user = await getScheduleHandler.Handle(
                    getScheduleQuery,
                    cancellationToken
                );
                scheduleSettings.Schedule = user.Value!;
            }
            return scheduleSettings;
        }
    }
}
