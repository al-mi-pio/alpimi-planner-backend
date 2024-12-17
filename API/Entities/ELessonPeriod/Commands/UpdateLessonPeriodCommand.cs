using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonPeriod.DTO;
using AlpimiAPI.Entities.ELessonPeriod.Queries;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ELessonPeriod.Commands
{
    public record UpdateLessonPeriodCommand(
        Guid Id,
        UpdateLessonPeriodDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<LessonPeriod?>;

    public class UpdateLessonPeriodHandler
        : IRequestHandler<UpdateLessonPeriodCommand, LessonPeriod?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public UpdateLessonPeriodHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<LessonPeriod?> Handle(
            UpdateLessonPeriodCommand request,
            CancellationToken cancellationToken
        )
        {
            LessonPeriod? originalLessonPeriod;
            switch (request.Role)
            {
                case "Admin":
                    originalLessonPeriod = await _dbService.Get<LessonPeriod?>(
                        @"
                            SELECT 
                            [Id], [Start], [ScheduleSettingsId]
                            FROM [LessonPeriod] 
                            WHERE [Id]=@Id;",
                        request
                    );
                    break;
                default:
                    originalLessonPeriod = await _dbService.Get<LessonPeriod?>(
                        @"
                            SELECT 
                            lp.[Id], lp.[Start], lp.[ScheduleSettingsId]
                            FROM [LessonPeriod] lp
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id] = ss.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND lp.[Id] = @Id;",
                        request
                    );
                    break;
            }

            if (originalLessonPeriod == null)
            {
                return null;
            }

            request.dto.Start = request.dto.Start ?? originalLessonPeriod.Start;

            GetScheduleSettingsHandler getScheduleSettingsHandler = new GetScheduleSettingsHandler(
                _dbService
            );
            GetScheduleSettingsQuery getScheduleSettingsQuery = new GetScheduleSettingsQuery(
                originalLessonPeriod!.ScheduleSettingsId,
                new Guid(),
                "Admin"
            );
            ActionResult<ScheduleSettings?> scheduleSettings =
                await getScheduleSettingsHandler.Handle(
                    getScheduleSettingsQuery,
                    cancellationToken
                );

            GetAllLessonPeriodByScheduleHandler getAllLessonPeriodByScheduleHandler =
                new GetAllLessonPeriodByScheduleHandler(_dbService, _str);
            GetAllLessonPeriodByScheduleQuery getAllLessonPeriodByScheduleQuery =
                new GetAllLessonPeriodByScheduleQuery(
                    scheduleSettings.Value!.ScheduleId,
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
                foreach (var oneLessonPeriod in allLessonsPeriods.Value.Item1)
                {
                    if (
                        request.dto.Start
                            > oneLessonPeriod.Start.AddMinutes(-scheduleSettings.Value.SchoolHour)
                        && request.dto.Start
                            < oneLessonPeriod.Start.AddMinutes(scheduleSettings.Value.SchoolHour)
                    )
                    {
                        throw new ApiErrorException(
                            [new ErrorObject(_str["timeOverlap", "LessonPeriod"])]
                        );
                    }
                }
            }

            var lessonPeriod = await _dbService.Update<LessonPeriod?>(
                $@"
                    UPDATE [LessonPeriod] 
                    SET
                    [Start] = @Start
                    OUTPUT
                    INSERTED.[Id],
                    INSERTED.[Start],
                    INSERTED.[ScheduleSettingsId]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            lessonPeriod!.ScheduleSettings = scheduleSettings.Value;

            return lessonPeriod;
        }
    }
}
