using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ELessonPeriod.DTO;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ELessonPeriod.Commands
{
    public record DeleteLessonPeriodCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteLessonPeriodHandler : IRequestHandler<DeleteLessonPeriodCommand>
    {
        private readonly IDbService _dbService;

        public DeleteLessonPeriodHandler(IDbService dbService)
        {
            _dbService = dbService;
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
                    await _dbService.Delete(
                        @"
                            DELETE [LessonPeriod] 
                            WHERE [Id] = @Id;",
                        request
                    );
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
                    await _dbService.Delete(
                        @"
                            DELETE lp
                            FROM [LessonPeriod] lp
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = lp.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id] = ss.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND lp.[Id] = @Id;",
                        request
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
    }
}
