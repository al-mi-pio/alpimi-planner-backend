using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.EScheduleSettings.Queries
{
    public record GetScheduleSettingsQuery(Guid Id, Guid FilteredId, string Role)
        : IRequest<ScheduleSettings?>;

    public class GetScheduleSettingsHandler
        : IRequestHandler<GetScheduleSettingsQuery, ScheduleSettings?>
    {
        private readonly IDbService _dbService;

        public GetScheduleSettingsHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<ScheduleSettings?> Handle(
            GetScheduleSettingsQuery request,
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
                            [Id], [SchoolHour], [SchoolYearStart], [SchoolYearEnd], [ScheduleId], [SchoolDays], [IsPublic]
                            FROM [ScheduleSettings] 
                            WHERE [Id] = @Id OR [ScheduleId] = @Id;",
                        request
                    );
                    break;
                case "User":
                    scheduleSettings = await _dbService.Get<ScheduleSettings?>(
                        @"
                            SELECT 
                            ss.[Id], [SchoolHour], [SchoolYearStart], [SchoolYearEnd], [ScheduleId], [SchoolDays], [IsPublic]
                            FROM [ScheduleSettings] ss 
                            JOIN [Schedule] s ON s.[Id] = ss.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND (ss.[Id] = @Id OR [ScheduleId] = @Id);",
                        request
                    );
                    break;
                default:
                    scheduleSettings = await _dbService.Get<ScheduleSettings?>(
                        @"
                            SELECT 
                            [Id], [SchoolHour], [SchoolYearStart], [SchoolYearEnd], [ScheduleId], [SchoolDays], [IsPublic]
                            FROM [ScheduleSettings] 
                            WHERE [IsPublic] = 'TRUE' AND ([Id] = @Id OR [ScheduleId] = @Id)",
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
