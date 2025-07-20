using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule.DTO;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiAPI.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ESchedule.Commands
{
    public record UpdateScheduleCommand(
        Guid Id,
        UpdateScheduleDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Schedule?>;

    public class UpdateScheduleHandler : IRequestHandler<UpdateScheduleCommand, Schedule?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public UpdateScheduleHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<Schedule?> Handle(
            UpdateScheduleCommand request,
            CancellationToken cancellationToken
        )
        {
            GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
            GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<Schedule?> originalSchedule = await getScheduleHandler.Handle(
                getScheduleQuery,
                cancellationToken
            );

            if (originalSchedule.Value == null)
            {
                return null;
            }

            if (request.dto.Name != null)
            {
                AllowedCharacterTypes[]? allowedCharacterTypesScheduleName =
                    Configuration.GetAllowedCharacterTypesForScheduleName();

                if (!CharacterFilter.Allowed(request.dto.Name, allowedCharacterTypesScheduleName))
                {
                    throw new ApiErrorException(
                        [
                            new ErrorObject(
                                _str[
                                    "cantContain",
                                    "Name",
                                    string.Join(", ", allowedCharacterTypesScheduleName!)
                                ]
                            )
                        ]
                    );
                }

                var scheduleName = await _dbService.Get<Schedule?>(
                    $@"
                       SELECT
                       [Id], [Name], [UserId] 
                       FROM [Schedule] 
                       WHERE [Name] = @Name;",
                    request.dto
                );

                if (scheduleName != null)
                {
                    throw new ApiErrorException(
                        [
                            new FieldErrorObject(
                                "name",
                                _str["alreadyExists", _strFields["Schedule"], request.dto.Name]
                            )
                        ]
                    );
                }
            }

            request.dto.Name = request.dto.Name ?? originalSchedule.Value.Name;

            var schedule = await _dbService.Update<Schedule?>(
                $@"
                    UPDATE [Schedule] 
                    SET 
                    [Name] = @Name 
                    OUTPUT
                    INSERTED.[Id], 
                    INSERTED.[Name], 
                    INSERTED.[UserId]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            schedule!.User = originalSchedule.Value.User;

            return schedule;
        }
    }
}
