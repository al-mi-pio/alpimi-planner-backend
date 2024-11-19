﻿using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.EScheduleSettings.Queries;
using AlpimiAPI.Responses;
using alpimi_planner_backend.API.Locales;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EScheduleSettings.Commands
{
    public record UpdateScheduleSettingsCommand(
        Guid ScheduleId,
        int? SchoolHour,
        DateTime? SchoolYearStart,
        DateTime? SchoolYearEnd,
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
            GetScheduleSettingsByScheduleIdHandler getScheduleSettingsByScheduleIdHandler =
                new GetScheduleSettingsByScheduleIdHandler(_dbService);
            GetScheduleSettingsByScheduleIdQuery getScheduleSettingsByScheduleIdQuery =
                new GetScheduleSettingsByScheduleIdQuery(request.ScheduleId, new Guid(), "Admin");
            ActionResult<ScheduleSettings?> scheduleSettingsSchoolYearPeriod =
                await getScheduleSettingsByScheduleIdHandler.Handle(
                    getScheduleSettingsByScheduleIdQuery,
                    cancellationToken
                );
            if (
                (request.SchoolYearStart ?? scheduleSettingsSchoolYearPeriod.Value!.SchoolYearStart)
                > (request.SchoolYearEnd ?? scheduleSettingsSchoolYearPeriod.Value!.SchoolYearEnd)
            )
            {
                throw new ApiErrorException([new ErrorObject(_str["scheduleDate"])]);
            }

            ScheduleSettings? scheduleSettings;
            switch (request.Role)
            {
                case "Admin":
                    scheduleSettings = await _dbService.Update<ScheduleSettings?>(
                        @"
                            UPDATE [ScheduleSettings] 
                            SET [SchoolHour]=CASE WHEN @SchoolHour IS NOT NULL THEN @SchoolHour ELSE [SchoolHour] END,
                            [SchoolYearStart]=CASE WHEN @SchoolYearStart IS NOT NULL THEN @SchoolYearStart ELSE [SchoolYearStart] END, 
                            [SchoolYearEnd]=CASE WHEN @SchoolYearEnd IS NOT NULL THEN @SchoolYearEnd ELSE [SchoolYearEnd] END 
                            OUTPUT INSERTED.[Id], INSERTED.[SchoolHour], INSERTED.[SchoolYearStart], INSERTED.[SchoolYearEnd], INSERTED.[ScheduleId]
                            WHERE [ScheduleId]=@ScheduleId;",
                        request
                    );
                    break;
                default:
                    scheduleSettings = await _dbService.Update<ScheduleSettings?>(
                        @"
                            WITH UpdatedSettings AS (
                            SELECT ss.*
                            FROM [ScheduleSettings] ss
                            INNER JOIN [Schedule] s ON s.[Id] = ss.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND ss.[ScheduleId] = @ScheduleId)
                            UPDATE UpdatedSettings
                            SET [SchoolHour] = CASE WHEN @SchoolHour IS NOT NULL THEN @SchoolHour ELSE [SchoolHour] END,
                            [SchoolYearStart] = CASE WHEN @SchoolYearStart IS NOT NULL THEN @SchoolYearStart ELSE [SchoolYearStart] END,
                            [SchoolYearEnd] = CASE WHEN @SchoolYearEnd IS NOT NULL THEN @SchoolYearEnd ELSE [SchoolYearEnd] END
                            OUTPUT INSERTED.[Id], INSERTED.[SchoolHour], INSERTED.[SchoolYearStart], INSERTED.[SchoolYearEnd], INSERTED.[ScheduleId];",
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