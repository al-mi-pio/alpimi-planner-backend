using AlpimiAPI.Database;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.EScheduleSettings.Queries;
using AlpimiAPI.Responses;
using alpimi_planner_backend.API.Locales;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EDayOff.Commands
{
    public record CreateDayOffCommand(
        Guid Id,
        string Name,
        DateTime Date,
        int? numberOfDays,
        Guid ScheduleId,
        Guid FilteredId,
        string Role
    ) : IRequest<Guid>;

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
            DateTime to = new DateTime();
            if (request.numberOfDays == null)
            {
                to = request.Date;
            }
            else if (request.numberOfDays < 1)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["badParameter", "NumberOfDays"])]
                );
            }
            else
            {
                to = request.Date.AddDays(Convert.ToDouble(request.numberOfDays - 1));
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
                request.Date < scheduleSettings.Value!.SchoolYearStart
                || request.Date > scheduleSettings.Value.SchoolYearEnd
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
                @"
                    INSERT INTO [DayOff] ([Id],[Name],[From],[To],[ScheduleSettingsId])
                    OUTPUT INSERTED.Id                    
                    VALUES (@Id,@Name,@Date,'"
                    + to.ToString("yyyy-MM-dd HH:mm:ss")
                    + "','"
                    + scheduleSettings.Value.Id
                    + "');",
                request
            );

            return insertedId;
        }
    }
}
