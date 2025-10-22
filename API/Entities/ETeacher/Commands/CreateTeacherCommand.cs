using System.Text.RegularExpressions;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Entities.ETeacher.DTO;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ETeacher.Commands
{
    public record CreateTeacherCommand(Guid Id, CreateTeacherDTO dto, Guid FilteredId, string Role)
        : IRequest<Guid>;

    public class CreateTeacherHandler : IRequestHandler<CreateTeacherCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public CreateTeacherHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<Guid> Handle(
            CreateTeacherCommand request,
            CancellationToken cancellationToken
        )
        {
            GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
            GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                request.dto.ScheduleId!.Value,
                request.FilteredId,
                request.Role
            );
            ActionResult<Schedule?> schedule = await getScheduleHandler.Handle(
                getScheduleQuery,
                cancellationToken
            );

            if (schedule.Value == null)
            {
                throw new ApiErrorException(
                    [
                        new ErrorObject(
                            _str["resourceNotFound", _strFields["Schedule"], request.dto.ScheduleId]
                        )
                    ]
                );
            }

            if (!Regex.IsMatch(request.dto.Email!, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ApiErrorException(
                    [new FieldErrorObject("email", _str["badParameter", "Email"])]
                );
            }

            var teacherEmail = await _dbService.Get<Teacher>(
                @"
                    SELECT 
                    [Id]
                    FROM [Teacher] 
                    WHERE [Email] = @Email AND [ScheduleId] = @ScheduleId;",
                request.dto
            );

            if (teacherEmail != null)
            {
                throw new ApiErrorException(
                    [
                        new FieldErrorObject(
                            "email",
                            _str["alreadyExists", "Teacher", request.dto.Email]
                        )
                    ]
                );
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [Teacher] 
                    ([Id], [Name], [Surname], [Email], [ScheduleId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @Name,
                    @Surname,
                    @Email,
                    @ScheduleId);",
                request.dto
            );

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = insertedId,
                AffectedEntity = "Teacher",
                Command = "Create",
                ReversaleDTO = null,
                CollisionChecked = false,
                ScheduleId = schedule.Value.Id
            };
            AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
            await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);

            return insertedId;
        }
    }
}
