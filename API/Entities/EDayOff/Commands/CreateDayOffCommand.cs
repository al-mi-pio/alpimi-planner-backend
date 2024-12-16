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
    public record CreateDayOffCommand(Guid Id, CreateDayOffDTO dto, Guid FilteredId, string Role)
        : IRequest<Guid>;

    public class CreateDayOffHandler : IRequestHandler<CreateDayOffCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateDayOffHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid> Handle(
            CreateDayOffCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.dto.To == null)
            {
                request.dto.To = request.dto.From;
            }
            else if (request.dto.To < request.dto.From)
            {
                throw new ApiErrorException([new ErrorObject(_str["scheduleDate"])]);
            }

            GetScheduleSettingsByScheduleIdHandler getScheduleSettingsByScheduleIdHandler =
                new GetScheduleSettingsByScheduleIdHandler(_dbService);
            GetScheduleSettingsByScheduleIdQuery getScheduleSettingsByScheduleIdQuery =
                new GetScheduleSettingsByScheduleIdQuery(
                    request.dto.ScheduleId,
                    request.FilteredId,
                    request.Role
                );

            ActionResult<ScheduleSettings?> scheduleSettings =
                await getScheduleSettingsByScheduleIdHandler.Handle(
                    getScheduleSettingsByScheduleIdQuery,
                    cancellationToken
                );
            if (scheduleSettings.Value == null)
            {
                throw new ApiErrorException(
                    [
                        new ErrorObject(
                            _str["resourceNotFound", "ScheduleSettings", request.dto.ScheduleId]
                        )
                    ]
                );
            }

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
                                scheduleSettings.Value!.SchoolYearStart.ToString("dd/MM/yyyy"),
                                scheduleSettings.Value.SchoolYearEnd.ToString("dd/MM/yyyy")
                            ]
                        )
                    ]
                );
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [DayOff] 
                    ([Id], [Name], [From], [To], [ScheduleSettingsId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',
                    @Name,
                    @From,
                    @To,
                    '{scheduleSettings.Value.Id}');",
                request.dto
            );

            return insertedId;
        }
    }
}
