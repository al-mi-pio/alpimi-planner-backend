using AlpimiAPI.Database;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.Queries;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using alpimi_planner_backend.API.Locales;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EDayOff.Commands
{
    public record CreateMultipleDayOffCommand(
        Guid Id,
        string Name,
        DateTime From,
        DateTime To,
        Guid ScheduleId,
        Guid FilteredId,
        string Role
    ) : IRequest;

    public class CreateMultipleDayOffHandler : IRequestHandler<CreateMultipleDayOffCommand>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateMultipleDayOffHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task Handle(
            CreateMultipleDayOffCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.From > request.To)
            {
                throw new ApiErrorException([new ErrorObject(_str["scheduleDate"])]);
            }
            GetScheduleSettingsByScheduleIdHandler getScheduleSettingsByScheduleIdHandler =
                new GetScheduleSettingsByScheduleIdHandler(_dbService);
            GetScheduleSettingsByScheduleIdQuery getScheduleSettingsByScheduleIdQuery =
                new GetScheduleSettingsByScheduleIdQuery(
                    request.ScheduleId,
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
                    [new ErrorObject(_str["notFound", "ScheduleSettings"])]
                );
            }
            if (
                request.From < scheduleSettings.Value!.SchoolYearStart
                || request.To > scheduleSettings.Value.SchoolYearEnd
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
            for (DateTime date = request.From; date <= request.To; date = date.AddDays(1))
            {
                CreateDayOffHandler createDayOffHandler = new CreateDayOffHandler(_dbService, _str);
                CreateDayOffCommand createDayOffCommand = new CreateDayOffCommand(
                    Guid.NewGuid(),
                    request.Name,
                    date,
                    request.ScheduleId,
                    request.FilteredId,
                    request.Role
                );

                ActionResult<Guid> pp = await createDayOffHandler.Handle(
                    createDayOffCommand,
                    cancellationToken
                );
            }
        }
    }
}
