using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonPeriod.DTO;
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
                            [Id],[Start],[Finish],[ScheduleSettingsId]
                            FROM [LessonPeriod] 
                            WHERE [Id]=@Id;",
                        request
                    );
                    break;
                default:
                    originalLessonPeriod = await _dbService.Get<LessonPeriod?>(
                        @"
                            SELECT 
                            lp.[Id],lp.[Start],lp.[Finish],lp.[ScheduleSettingsId]
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
            request.dto.Finish = request.dto.Finish ?? originalLessonPeriod.Finish;

            if (request.dto.Start > request.dto.Finish)
            {
                throw new ApiErrorException([new ErrorObject(_str["scheduleDate"])]);
            }

            var lessonPeriodOverlap = await _dbService.GetAll<LessonPeriod>(
                $@"
                    SELECT 
                    lp.[Id]
                    FROM [LessonPeriod] lp
                    INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                    INNER JOIN [Schedule] s ON s.[Id]=ss.[ScheduleId]
                    WHERE s.[UserId] = '{request.FilteredId}' AND ss.[Id] = '{originalLessonPeriod.ScheduleSettingsId}'
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

            var lessonPeriod = await _dbService.Update<LessonPeriod?>(
                $@"
                    UPDATE [LessonPeriod] 
                    SET
                    [Start] = @Start, [Finish] = @Finish
                    OUTPUT
                    INSERTED.[Id],
                    INSERTED.[Start],
                    INSERTED.[Finish],
                    INSERTED.[ScheduleSettingsId]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            GetScheduleSettingsHandler getScheduleSettingsHandler = new GetScheduleSettingsHandler(
                _dbService
            );
            GetScheduleSettingsQuery getScheduleSettingsQuery = new GetScheduleSettingsQuery(
                lessonPeriod!.ScheduleSettingsId,
                new Guid(),
                "Admin"
            );
            ActionResult<ScheduleSettings?> toInsertScheduleSettings =
                await getScheduleSettingsHandler.Handle(
                    getScheduleSettingsQuery,
                    cancellationToken
                );
            lessonPeriod.ScheduleSettings = toInsertScheduleSettings.Value!;

            return lessonPeriod;
        }
    }
}
