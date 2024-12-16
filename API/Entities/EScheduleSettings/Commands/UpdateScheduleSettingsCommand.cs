using System.Text.RegularExpressions;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.ELessonBlock;
using AlpimiAPI.Entities.ELessonPeriod;
using AlpimiAPI.Entities.ELessonPeriod.Queries;
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
            List<ErrorObject> errors = new List<ErrorObject>();
            if (request.dto.SchoolHour != null)
            {
                if (request.dto.SchoolHour < 1 || request.dto.SchoolHour > 1440)
                {
                    errors.Add(new ErrorObject(_str["badParameter", "SchoolHour"]));
                }
            }

            if (request.dto.SchoolDays != null)
            {
                if (
                    !Regex.IsMatch(request.dto.SchoolDays, @"^[01]+$")
                    || request.dto.SchoolDays.Length != 7
                )
                {
                    errors.Add(new ErrorObject(_str["badParameter", "SchoolDays"]));
                }
            }
            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
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
                request.dto.SchoolYearEnd ?? originalScheduleSettings.Value.SchoolYearEnd;
            request.dto.SchoolDays =
                request.dto.SchoolDays ?? originalScheduleSettings.Value.SchoolDays;

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
                    AND (do.[To] > @SchoolYearEnd OR do.[From] < @SchoolYearStart);",
                request.dto
            );
            if (daysOffOutOfRange!.Any())
            {
                throw new ApiErrorException([new ErrorObject(_str["outOfRange", "DayOff"])]);
            }

            var lessonBlocksOutOfRange = await _dbService.GetAll<LessonBlock>(
                $@"
                    SELECT
                    lb.[Id] 
                    FROM [LessonBlock] lb
                    INNER JOIN [Lesson] l ON l.[Id] = lb.[LessonId]
                    INNER JOIN [LessonType] lt ON lt.[Id] = l.[LessonTypeId]
                    WHERE lt.[ScheduleId] = '{request.ScheduleId}'
                    AND (lb.[LessonDate] > @SchoolYearEnd OR lb.[LessonDate] < @SchoolYearStart);",
                request.dto
            );
            if (lessonBlocksOutOfRange!.Any())
            {
                throw new ApiErrorException([new ErrorObject(_str["outOfRange", "LessonBlock"])]);
            }

            GetAllLessonPeriodByScheduleHandler getAllLessonPeriodByScheduleHandler =
                new GetAllLessonPeriodByScheduleHandler(_dbService, _str);
            GetAllLessonPeriodByScheduleQuery getAllLessonPeriodByScheduleQuery =
                new GetAllLessonPeriodByScheduleQuery(
                    originalScheduleSettings.Value.ScheduleId,
                    request.FilteredId,
                    request.Role,
                    new PaginationParams(1440, 0, "Start", "ASC")
                );
            ActionResult<(IEnumerable<LessonPeriod>?, int)> allLessonsPeriods =
                await getAllLessonPeriodByScheduleHandler.Handle(
                    getAllLessonPeriodByScheduleQuery,
                    cancellationToken
                );

            if (allLessonsPeriods.Value.Item1 != null)
            {
                for (int i = 0; i < allLessonsPeriods.Value.Item1.Count() - 1; i++)
                {
                    if (
                        allLessonsPeriods
                            .Value.Item1.ElementAt(i)
                            .Start.AddMinutes(request.dto.SchoolHour.Value)
                        > allLessonsPeriods.Value.Item1.ElementAt(i + 1).Start
                    )
                    {
                        throw new ApiErrorException(
                            [new ErrorObject(_str["timeOverlap", "LessonPeriod"])]
                        );
                    }
                }
            }

            var scheduleSettings = await _dbService.Update<ScheduleSettings?>(
                $@"
                    UPDATE [ScheduleSettings] 
                    SET 
                    [SchoolHour] = @SchoolHour,
                    [SchoolYearStart] = @SchoolYearStart, 
                    [SchoolYearEnd] = @SchoolYearEnd, 
                    [SchoolDays] = @SchoolDays
                    OUTPUT 
                    INSERTED.[Id], 
                    INSERTED.[SchoolHour], 
                    INSERTED.[SchoolYearStart], 
                    INSERTED.[SchoolYearEnd], 
                    INSERTED.[SchoolDays],
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
