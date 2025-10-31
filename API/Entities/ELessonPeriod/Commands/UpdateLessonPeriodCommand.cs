using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
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
        private readonly IStringLocalizer<Fields> _strFields;

        public UpdateLessonPeriodHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
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

            UpdateLessonPeriodDTO reversaleDTOLessonPeriod = new UpdateLessonPeriodDTO
            {
                Start = originalLessonPeriod.Start,
            };

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
                new GetAllLessonPeriodByScheduleHandler(_dbService, _str, _strFields);
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
                            >= (
                                oneLessonPeriod.Start.AddMinutes(-scheduleSettings.Value.SchoolHour)
                                > oneLessonPeriod.Start
                                    ? TimeOnly.MinValue
                                    : oneLessonPeriod.Start.AddMinutes(
                                        -scheduleSettings.Value.SchoolHour
                                    )
                            )
                        && request.dto.Start
                            < (
                                oneLessonPeriod.Start.AddMinutes(scheduleSettings.Value.SchoolHour)
                                < oneLessonPeriod.Start
                                    ? TimeOnly.MaxValue
                                    : oneLessonPeriod.Start.AddMinutes(
                                        scheduleSettings.Value.SchoolHour
                                    )
                            )
                    )
                    {
                        throw new ApiErrorException(
                            [
                                new FieldErrorObject(
                                    "start",
                                    _str["timeOverlap", _strFields["LessonPeriod"]]
                                )
                            ]
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

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = request.Id,
                AffectedEntity = "LessonPeriod",
                Command = "Patch",
                ReversaleDTO = JsonSerializer.Serialize(reversaleDTOLessonPeriod),
                CollisionChecked = false,
                ScheduleId = scheduleSettings.Value.ScheduleId,
            };
            AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
            await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);

            return lessonPeriod;
        }
    }
}
