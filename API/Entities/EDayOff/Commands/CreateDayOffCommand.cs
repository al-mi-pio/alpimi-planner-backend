using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.DTO;
using AlpimiAPI.Entities.EHistory.DTO;
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
        private readonly IStringLocalizer<Fields> _strFields;

        public CreateDayOffHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
            _strFields = strFields;
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

            GetScheduleSettingsHandler getScheduleSettingsHandler = new GetScheduleSettingsHandler(
                _dbService
            );
            GetScheduleSettingsQuery getScheduleSettingsQuery = new GetScheduleSettingsQuery(
                request.dto.ScheduleId!.Value,
                request.FilteredId,
                request.Role
            );
            ActionResult<ScheduleSettings?> scheduleSettings =
                await getScheduleSettingsHandler.Handle(
                    getScheduleSettingsQuery,
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

            List<ErrorObject> errors = new List<ErrorObject>();

            var dayOffName = await _dbService.Get<DayOff>(
                @"
                    SELECT 
                    do.[Id]
                    FROM [DayOff] do
                    INNER JOIN [ScheduleSettings] ss on ss.[Id] = do.[ScheduleSettingsId]
                    WHERE [Name] = @Name AND ss.[ScheduleId] = @ScheduleId;",
                request.dto
            );
            if (dayOffName != null)
            {
                errors.Add(
                    new FieldErrorObject(
                        "name",
                        _str["alreadyExists", _strFields["DayOff"], request.dto.Name!]
                    )
                );
            }

            if (
                request.dto.From < scheduleSettings.Value!.SchoolYearStart
                || request.dto.To > scheduleSettings.Value.SchoolYearEnd
            )
            {
                errors.Add(
                    new ErrorObject(
                        _str[
                            "dateOutOfRange",
                            scheduleSettings.Value!.SchoolYearStart.ToString("dd/MM/yyyy"),
                            scheduleSettings.Value.SchoolYearEnd.ToString("dd/MM/yyyy")
                        ]
                    )
                );
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
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

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = insertedId,
                AffectedEntity = "DayOff",
                Command = "Create",
                ReversaleDTO = null,
                CollisionChecked = false,
                ScheduleId = scheduleSettings.Value.ScheduleId
            };
            AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
            await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);

            return insertedId;
        }
    }
}
