using System.Text.RegularExpressions;
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
    public record CreateScheduleCommand(
        Guid Id,
        Guid UserId,
        Guid ScheduleSettingsId,
        CreateScheduleDTO dto
    ) : IRequest<Guid>;

    public class CreateScheduleHandler : IRequestHandler<CreateScheduleCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateScheduleHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid> Handle(
            CreateScheduleCommand request,
            CancellationToken cancellationToken
        )
        {
            List<ErrorObject> errors = new List<ErrorObject>();
            if (request.dto.SchoolHour < 1 || request.dto.SchoolHour > 1440)
            {
                errors.Add(new ErrorObject(_str["badParameter", "SchoolHour"]));
            }

            if (
                !Regex.IsMatch(request.dto.SchoolDays, @"^[01]+$")
                || request.dto.SchoolDays.Length != 7
            )
            {
                errors.Add(new ErrorObject(_str["badParameter", "SchoolDays"]));
            }

            AllowedCharacterTypes[]? allowedCharacterTypesScheduleName =
                Configuration.GetAllowedCharacterTypesForScheduleName();

            if (!CharacterFilter.Allowed(request.dto.Name, allowedCharacterTypesScheduleName))
            {
                errors.Add(
                    new ErrorObject(
                        _str[
                            "cantContain",
                            "Name",
                            string.Join(", ", allowedCharacterTypesScheduleName!)
                        ]
                    )
                );
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
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
                    [new ErrorObject(_str["alreadyExists", "Schedule", request.dto.Name])]
                );
            }

            if (request.dto.SchoolYearStart > request.dto.SchoolYearEnd)
            {
                throw new ApiErrorException([new ErrorObject(_str["scheduleDate"])]);
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [Schedule] 
                    ([Id], [Name], [UserId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @Name,
                    '{request.UserId}');",
                request.dto
            );

            await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [ScheduleSettings] 
                    ([Id], [SchoolHour], [SchoolYearStart], [SchoolYearEnd], [SchoolDays], [IsPublic], [ScheduleId])
                    OUTPUT 
                    INSERTED.Id
                    VALUES (
                    '{request.ScheduleSettingsId}',
                    @SchoolHour, 
                    @SchoolYearStart, 
                    @SchoolYearEnd,
                    @SchoolDays,
                    'FALSE',
                    '{request.Id}');",
                request.dto
            );

            return insertedId;
        }
    }
}
