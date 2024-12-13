using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.EScheduleSettings.DTO;
using AlpimiAPI.Entities.EScheduleSettings.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EScheduleSettings.Commands
{
    public record UpdateScheduleSettingsCommand(
        Guid ScheduleId,
        UpdateScheduleSettingsDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<ScheduleSettings?>;

    public class UpdateScheduleSettingsHandler
        : IRequestHandler<UpdateScheduleSettingsCommand, ScheduleSettings?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public UpdateScheduleSettingsHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<ScheduleSettings?> Handle(
            UpdateScheduleSettingsCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.SchoolHour != null)
            {
                if (request.dto.SchoolHour < 1)
                {
                    throw new ApiErrorException(
                        [new ErrorObject(_str["badParameter", "SchoolHour"])]
                    );
                }
            }

            GetScheduleSettingsByScheduleIdHandler getScheduleSettingsByScheduleIdHandler =
                new GetScheduleSettingsByScheduleIdHandler(_dbService);
            GetScheduleSettingsByScheduleIdQuery getScheduleSettingsByScheduleIdQuery =
                new GetScheduleSettingsByScheduleIdQuery(
                    request.ScheduleId,
                    request.FilteredId,
                    request.Role
                );
            ActionResult<ScheduleSettings?> originalScheduleSettings =
                await getScheduleSettingsByScheduleIdHandler.Handle(
                    getScheduleSettingsByScheduleIdQuery,
                    cancellationToken
                );

            if (originalScheduleSettings.Value == null)
            {
                return null;
            }

            request.dto.SchoolHour =
                request.dto.SchoolHour ?? originalScheduleSettings.Value.SchoolHour;
            request.dto.SchoolYearStart =
                request.dto.SchoolYearStart ?? originalScheduleSettings.Value.SchoolYearStart;
            request.dto.SchoolYearEnd =
                request.dto.SchoolYearEnd ?? originalScheduleSettings.Value?.SchoolYearEnd;

            if (request.dto.SchoolYearStart > request.dto.SchoolYearEnd)
            {
                throw new ApiErrorException([new ErrorObject(_str["scheduleDate"])]);
            }

            var daysOffOutOfRange = await _dbService.GetAll<DayOff>(
                $@"
                    SELECT
                    do.[Id] 
                    FROM [DayOff] do
                    INNER JOIN [ScheduleSettings] ss ON ss.[Id] = do.[ScheduleSettingsId]
                    WHERE ss.[ScheduleId] = '{request.ScheduleId}'
                    AND (do.[To] > @SchoolYearEnd OR do.[From] < @SchoolYearStart)",
                request.dto
            );

            if (daysOffOutOfRange!.Any())
            {
                throw new ApiErrorException([new ErrorObject(_str["outOfRange"])]);
            }

            var scheduleSettings = await _dbService.Update<ScheduleSettings?>(
                $@"
                    UPDATE [ScheduleSettings] 
                    SET 
                    [SchoolHour] = @SchoolHour, [SchoolYearStart] = @SchoolYearStart, [SchoolYearEnd] = @SchoolYearEnd
                    OUTPUT 
                    INSERTED.[Id], 
                    INSERTED.[SchoolHour], 
                    INSERTED.[SchoolYearStart], 
                    INSERTED.[SchoolYearEnd], 
                    INSERTED.[ScheduleId]
                    WHERE [ScheduleId] = '{request.ScheduleId}';",
                request.dto
            );

            GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
            GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                scheduleSettings!.ScheduleId,
                new Guid(),
                "Admin"
            );
            ActionResult<Schedule?> user = await getScheduleHandler.Handle(
                getScheduleQuery,
                cancellationToken
            );
            scheduleSettings.Schedule = user.Value!;

            return scheduleSettings;
        }
    }
}
