using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.DTO;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.EDayOff.Commands
{
    public record DeleteDayOffCommand(Guid Id, Guid FilteredId, string Role) : IRequest;

    public class DeleteDayOffHandler : IRequestHandler<DeleteDayOffCommand>
    {
        private readonly IDbService _dbService;

        public DeleteDayOffHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task Handle(DeleteDayOffCommand request, CancellationToken cancellationToken)
        {
            DayOff? dayOff;
            switch (request.Role)
            {
                case "Admin":
                    dayOff = await _dbService.Get<DayOff?>(
                        @"
                            SELECT
                            [Id], [Name], [From], [To], [ScheduleSettingsId]
                            FROM [DayOff]
                            WHERE [Id] = @Id;",
                        request
                    );
                    await _dbService.Delete(
                        @"
                            DELETE [DayOff] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    dayOff = await _dbService.Get<DayOff?>(
                        @"
                            SELECT
                            do.[Id], [Name], [From], [To], [ScheduleSettingsId]
                            FROM [DayOff] do
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = do.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id] = ss.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND do.[Id] = @Id;;",
                        request
                    );
                    await _dbService.Delete(
                        @"
                            DELETE do
                            FROM [DayOff] do
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = do.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id] = ss.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND do.[Id] = @Id;",
                        request
                    );
                    break;
            }
            if (dayOff != null)
            {
                GetScheduleSettingsHandler getScheduleSettingsHandler =
                    new GetScheduleSettingsHandler(_dbService);
                GetScheduleSettingsQuery getScheduleSettingsQuery = new GetScheduleSettingsQuery(
                    dayOff.ScheduleSettingsId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<ScheduleSettings?> scheduleSettings =
                    await getScheduleSettingsHandler.Handle(
                        getScheduleSettingsQuery,
                        cancellationToken
                    );
                dayOff.ScheduleSettings = scheduleSettings.Value!;

                CreateDayOffDTO reversaleDTO = new CreateDayOffDTO
                {
                    Name = dayOff.Name,
                    From = dayOff.From,
                    To = dayOff.To,
                    ScheduleId = dayOff.ScheduleSettings.ScheduleId,
                };
                AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
                AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    AffectedEntityId = request.Id,
                    AffectedEntity = "DayOff",
                    Command = "Delete",
                    ReversaleDTO = JsonSerializer.Serialize(reversaleDTO),
                    CollisionChecked = true,
                    ScheduleId = reversaleDTO.ScheduleId!.Value,
                };
                AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
                await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);
            }
        }
    }
}
