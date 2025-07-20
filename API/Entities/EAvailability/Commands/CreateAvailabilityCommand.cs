using AlpimiAPI.Database;
using AlpimiAPI.Entities.EAvailability.DTO;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.ETeacher.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EAvailability.Commands
{
    public record CreateAvailabilityCommand(
        Guid Id,
        CreateAvailabilityDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Guid>;

    public class CreateAvailabilityHandler : IRequestHandler<CreateAvailabilityCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public CreateAvailabilityHandler(
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
            CreateAvailabilityCommand request,
            CancellationToken cancellationToken
        )
        {
            GetTeacherHandler getTeacherHandler = new GetTeacherHandler(_dbService);
            GetTeacherQuery getTeacherQuery = new GetTeacherQuery(
                request.dto.TeacherId!.Value,
                request.FilteredId,
                request.Role
            );
            ActionResult<Teacher?> teacher = await getTeacherHandler.Handle(
                getTeacherQuery,
                cancellationToken
            );
            if (teacher.Value == null)
            {
                throw new ApiErrorException(
                    [new ErrorObject(_str["resourceNotFound", "Teacher", request.dto.TeacherId])]
                );
            }

            var scheduleSettings = await _dbService.Get<ScheduleSettings?>(
                @"
                    SELECT DISTINCT
                    ss.[Id]
                    FROM [ScheduleSettings] ss
                    INNER JOIN [Teacher] t ON t.[ScheduleId] = ss.[ScheduleId]
                    WHERE t.[Id] = @TeacherId;",
                request.dto
            );

            var lessonPeriodCount = Utilities
                .LessonPeriodCount.Get(_dbService, scheduleSettings!.Id, cancellationToken)
                .Result;

            List<ErrorObject> errors = new List<ErrorObject>();
            if (request.dto.Start > request.dto.End)
            {
                errors.Add(new ErrorObject(_str["scheduleTime"]));
            }

            if (request.dto.Start < 1)
            {
                errors.Add(
                    new FieldErrorObject("start", _str["badParameter", _strFields["Start"]])
                );
            }

            if (request.dto.End > lessonPeriodCount)
            {
                errors.Add(new FieldErrorObject("end", _str["badParameter", _strFields["End"]]));
            }

            if (request.dto.WeekDay < 0 || request.dto.WeekDay > 6)
            {
                errors.Add(
                    new FieldErrorObject("weekDay", _str["badParameter", _strFields["WeekDay"]])
                );
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [Availability] 
                    ([Id], [WeekDay], [Start], [End], [TeacherId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @WeekDay,
                    @Start,
                    @End,
                    @TeacherId);",
                request.dto
            );

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = insertedId,
                AffectedEntity = "Availability",
                Command = "Create",
                ReversaleDTO = null,
                CollisionChecked = false,
                ScheduleId = teacher.Value.ScheduleId
            };
            AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
            await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);

            return insertedId;
        }
    }
}
