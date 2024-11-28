using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff;
using AlpimiAPI.Entities.ELessonPeriod.DTO;
using AlpimiAPI.Entities.ELessonPerioid;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.Queries;
using AlpimiAPI.Responses;
using alpimi_planner_backend.API.Locales;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;

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
            var scheduleSettings = await _dbService.Get<ScheduleSettings?>(
                @"
                    SELECT
                    ss.[Id], [SchoolHour], [SchoolYearStart], [SchoolYearEnd], [ScheduleId]
                    FROM [ScheduleSettings] ss
                    INNER JOIN [LessonPeriod] lp ON lp.[ScheduleSettingsId]=ss.[Id]
                    WHERE lp.[Id]=@Id ;",
                request
            );

            if (scheduleSettings == null)
            {
                return null;
            }

            var originalLessonPeriod = await _dbService.Get<LessonPeriod?>(
                @"
                    SELECT 
                    [Id],[Start],[Finish],[ScheduleSettingsId]
                    FROM [LessonPeriod] 
                    WHERE [Id]=@Id;",
                request
            );

            if (
                (request.dto.Start ?? originalLessonPeriod!.Start)
                > (request.dto.Finish ?? originalLessonPeriod!.Finish)
            )
            {
                throw new ApiErrorException([new ErrorObject(_str["scheduleDate"])]);
            }

            request.dto.Start = request.dto.Start ?? originalLessonPeriod!.Start;
            request.dto.Finish = request.dto.Finish ?? originalLessonPeriod!.Finish;

            var lessonPeriodOverlap = await _dbService.GetAll<LessonPeriod>(
                $@"
                    SELECT 
                    lp.[Id]
                    FROM [LessonPeriod] lp
                    INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                    INNER JOIN [Schedule] s ON s.[Id]=ss.[ScheduleId]
                    WHERE s.[UserId] = '{request.FilteredId}' AND ss.[Id] = '{originalLessonPeriod!.ScheduleSettingsId}'
                    AND lp.[Id] != '{request.Id}'
                    AND ((([Start] > @Start AND [Start] < @Finish) 
                    OR ([Finish] > @Start AND [Finish] < @Finish))
                    OR ([Start] = @Start AND [Finish] = @Finish)); ",
                request.dto
            );
            if (lessonPeriodOverlap!.Any())
            {
                throw new ApiErrorException([new ErrorObject(_str["timeOverlap"])]);
            }

            LessonPeriod? lessonPeriod;
            switch (request.Role)
            {
                case "Admin":
                    lessonPeriod = await _dbService.Update<LessonPeriod?>(
                        $@"
                            UPDATE [LessonPeriod] 
                            SET
                            [Start]=CASE WHEN @Start IS NOT NULL THEN @Start ELSE [Start] END,
                            [Finish]=CASE WHEN @Finish IS NOT NULL THEN @Finish ELSE [Finish] END
                            OUTPUT
                            INSERTED.[Id],
                            INSERTED.[Start],
                            INSERTED.[Finish],
                            INSERTED.[ScheduleSettingsId]
                            WHERE [Id] = '{request.Id}';",
                        request.dto
                    );
                    break;
                default:
                    lessonPeriod = await _dbService.Update<LessonPeriod?>(
                        $@"
                            UPDATE lp
                            SET 
                            [Start]=CASE WHEN @Start IS NOT NULL THEN @Start ELSE [Start] END,
                            [Finish]=CASE WHEN @Finish IS NOT NULL THEN @Finish ELSE [Finish] END
                            OUTPUT 
                            INSERTED.[Id],
                            INSERTED.[Start],
                            INSERTED.[Finish],
                            INSERTED.[ScheduleSettingsId]
                            FROM [LessonPeriod] lp
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id] = ss.[ScheduleId]
                            WHERE s.[UserId] = '{request.FilteredId}' AND lp.[Id] = '{request.Id}';",
                        request.dto
                    );
                    break;
            }

            if (lessonPeriod != null)
            {
                GetScheduleSettingsHandler getScheduleSettingsHandler =
                    new GetScheduleSettingsHandler(_dbService);
                GetScheduleSettingsQuery getScheduleSettingsQuery = new GetScheduleSettingsQuery(
                    lessonPeriod.ScheduleSettingsId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<ScheduleSettings?> toInsertScheduleSettings =
                    await getScheduleSettingsHandler.Handle(
                        getScheduleSettingsQuery,
                        cancellationToken
                    );
                lessonPeriod.ScheduleSettings = toInsertScheduleSettings.Value!;
            }

            return lessonPeriod;
        }
    }
}
