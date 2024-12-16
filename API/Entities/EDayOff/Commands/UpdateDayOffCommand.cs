using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.DTO;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EDayOff.Commands
{
    public record UpdateDayOffCommand(Guid Id, UpdateDayOffDTO dto, Guid FilteredId, string Role)
        : IRequest<DayOff?>;

    public class UpdateDayOffHandler : IRequestHandler<UpdateDayOffCommand, DayOff?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public UpdateDayOffHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<DayOff?> Handle(
            UpdateDayOffCommand request,
            CancellationToken cancellationToken
        )
        {
            DayOff? originalDayOff;
            switch (request.Role)
            {
                case "Admin":
                    originalDayOff = await _dbService.Get<DayOff?>(
                        @"
                            SELECT 
                            [Id], [Name], [From], [To], [ScheduleSettingsId]
                            FROM [DayOff] 
                            WHERE [Id]=@Id; ",
                        request
                    );
                    break;
                default:
                    originalDayOff = await _dbService.Get<DayOff?>(
                        @"
                            SELECT 
                            do.[Id], do.[Name], do.[From], do.[To], do.[ScheduleSettingsId]
                            FROM [DayOff] do
                            INNER JOIN [ScheduleSettings] ss ON ss.[Id] = do.[ScheduleSettingsId]
                            INNER JOIN [Schedule] s ON s.[Id] = ss.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND do.[Id] = @Id; ",
                        request
                    );
                    break;
            }

            if (originalDayOff == null)
            {
                return null;
            }

            request.dto.Name = request.dto.Name ?? originalDayOff.Name;
            request.dto.From = request.dto.From ?? originalDayOff.From;
            request.dto.To = request.dto.To ?? originalDayOff.To;

            if (request.dto.From > request.dto.To)
            {
                throw new ApiErrorException([new ErrorObject(_str["scheduleDate"])]);
            }

            GetScheduleSettingsHandler getScheduleSettingsHandler = new GetScheduleSettingsHandler(
                _dbService
            );
            GetScheduleSettingsQuery getScheduleSettingsQuery = new GetScheduleSettingsQuery(
                originalDayOff.ScheduleSettingsId,
                request.FilteredId,
                request.Role
            );
            ActionResult<ScheduleSettings?> scheduleSettings =
                await getScheduleSettingsHandler.Handle(
                    getScheduleSettingsQuery,
                    cancellationToken
                );

            if (
                request.dto.From < scheduleSettings.Value!.SchoolYearStart
                || request.dto.To > scheduleSettings.Value.SchoolYearEnd
            )
            {
                throw new ApiErrorException(
                    [
                        new ErrorObject(
                            _str[
                                "dateOutOfRange",
                                scheduleSettings.Value.SchoolYearStart.ToString("dd/MM/yyyy"),
                                scheduleSettings.Value.SchoolYearEnd.ToString("dd/MM/yyyy")
                            ]
                        )
                    ]
                );
            }

            var dayOff = await _dbService.Update<DayOff?>(
                $@"
                    UPDATE [DayOff] 
                    SET 
                    [Name] = @Name, [From] = @From, [To] = @To 
                    OUTPUT
                    INSERTED.[Id], 
                    INSERTED.[Name],
                    INSERTED.[From],
                    INSERTED.[To],
                    INSERTED.[ScheduleSettingsId]
                    WHERE [Id] = '{request.Id}'; ",
                request.dto
            );

            dayOff!.ScheduleSettings = scheduleSettings.Value!;

            return dayOff;
        }
    }
}
