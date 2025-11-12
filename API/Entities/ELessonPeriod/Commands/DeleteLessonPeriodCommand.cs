using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ELessonPeriod.DTO;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ELessonPeriod.Commands
{
    public record DeleteLessonPeriodCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteLessonPeriodHandler : IRequestHandler<DeleteLessonPeriodCommand>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public DeleteLessonPeriodHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task Handle(
            DeleteLessonPeriodCommand request,
            CancellationToken cancellationToken
        )
        {
            LessonPeriod? lessonPeriod;
            switch (request.Role)
            {
                case "Admin":
                    lessonPeriod = await _dbService.Get<LessonPeriod?>(
                        @"
                            SELECT
                            [Id], [Start], [ScheduleSettingsId]
                            FROM [LessonPeriod]
                            WHERE [Id] = @Id;",
                        request
                    );
                    if (lessonPeriod != null)
                    {
                        await LessonBlockCheck(_dbService, lessonPeriod, cancellationToken);
                        await _dbService.Delete(
                            @"
                            DELETE [LessonPeriod] 
                            WHERE [Id] = @Id;",
                            request
                        );
                    }
                    break;
                default:
                    lessonPeriod = await _dbService.Get<LessonPeriod?>(
                        @"
                            SELECT
                            lp.[Id], [Start], [ScheduleSettingsId]
                            FROM [LessonPeriod] lp
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id] = ss.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND lp.[Id] = @Id;",
                        request
                    );
                    if (lessonPeriod != null)
                    {
                        await LessonBlockCheck(_dbService, lessonPeriod, cancellationToken);
                        await _dbService.Delete(
                            @"
                            DELETE lp
                            FROM [LessonPeriod] lp
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id] = ss.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND lp.[Id] = @Id;",
                            request
                        );
                    }
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
                ActionResult<ScheduleSettings?> scheduleSettings =
                    await getScheduleSettingsHandler.Handle(
                        getScheduleSettingsQuery,
                        cancellationToken
                    );
                lessonPeriod.ScheduleSettings = scheduleSettings.Value!;

                CreateLessonPeriodDTO reversaleDTO = new CreateLessonPeriodDTO
                {
                    Start = lessonPeriod.Start,
                    ScheduleId = lessonPeriod.ScheduleSettings.ScheduleId,
                };
                AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
                AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    AffectedEntityId = request.Id,
                    AffectedEntity = "LessonPeriod",
                    Command = "Delete",
                    ReversaleDTO = JsonSerializer.Serialize(reversaleDTO),
                    CollisionChecked = false,
                    ScheduleId = reversaleDTO.ScheduleId!.Value,
                };
                AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
                await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);
            }
        }

        private async Task LessonBlockCheck(
            IDbService dbService,
            LessonPeriod lessonPeriod,
            CancellationToken cancellationToken
        )
        {
            var lessonPeriodCount = await LessonPeriodCount.Get(
                dbService,
                lessonPeriod.ScheduleSettingsId,
                cancellationToken
            );

            var lessonBlocksOnFinalLessonHour = await _dbService.GetAll<Guid>(
                $@"
                    SELECT
                    lb.[Id]
                    FROM[LessonBlock] lb
                    INNER JOIN [Lesson] l ON l.[Id] = lb.[LessonId]
                    INNER JOIN [LessonType] lt ON lt.[Id] = l.[LessonTypeId]
                    INNER JOIN [Schedule] s ON s.[Id] = lt.[ScheduleId]
                    INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                    WHERE ss.[Id] = @ScheduleSettingsId AND [LessonEnd] = {lessonPeriodCount - 1}",
                lessonPeriod
            );
            var avaibilityOnFinalLessonHour = await _dbService.GetAll<Guid>(
                $@" 
                    SELECT 
                    a.[Id]
                    FROM [Availability] a
                    INNER JOIN [Teacher] t ON t.[Id] = a.[TeacherId]
                    INNER JOIN [Schedule] s ON s.[Id]= t.[ScheduleId]
                    INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                    WHERE ss.[Id] = @ScheduleSettingsId AND a.[End] = {lessonPeriodCount - 1}",
                lessonPeriod
            );
            List<ErrorObject> errors = new List<ErrorObject>();
            if (lessonBlocksOnFinalLessonHour!.Any())
            {
                errors.Add(
                    new FieldErrorObject(
                        $"{{lessonBlock: {string.Join(", ", lessonBlocksOnFinalLessonHour!)} }}",
                        _str[
                            "notEnoughLessonPeriods",
                            _strFields["LessonPeriod"],
                            _strFields["LessonBlock"]
                        ]
                    )
                );
            }
            if (avaibilityOnFinalLessonHour!.Any())
            {
                errors.Add(
                    new FieldErrorObject(
                        $"{{availability: {string.Join(", ", avaibilityOnFinalLessonHour!)} }}",
                        _str[
                            "notEnoughLessonPeriods",
                            _strFields["LessonPeriod"],
                            _strFields["Availability"]
                        ]
                    )
                );
            }
            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }
        }
    }
}
