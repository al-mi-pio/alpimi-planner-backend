using System.Text.Json;
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
    public record UpdateAvailabilityCommand(
        Guid Id,
        UpdateAvailabilityDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Availability?>;

    public class UpdateAvailabilityHandler
        : IRequestHandler<UpdateAvailabilityCommand, Availability?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public UpdateAvailabilityHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<Availability?> Handle(
            UpdateAvailabilityCommand request,
            CancellationToken cancellationToken
        )
        {
            Availability? originalAvailability;
            switch (request.Role)
            {
                case "Admin":
                    originalAvailability = await _dbService.Get<Availability?>(
                        @"
                            SELECT 
                            [Id], [WeekDay], [Start], [End], [TeacherId]
                            FROM [Availability] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    originalAvailability = await _dbService.Get<Availability?>(
                        @"
                            SELECT 
                            a.[Id], a.[WeekDay], a.[Start], a.[End], a.[TeacherId]
                            FROM [Availability] a
                            INNER JOIN [Teacher] t on t.[Id] = a.[TeacherId]
                            INNER JOIN [Schedule] s on s.[Id] = t.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND a.[Id] = @Id;",
                        request
                    );
                    break;
            }

            if (originalAvailability == null)
            {
                return null;
            }

            UpdateAvailabilityDTO reversaleDTOAvailability = new UpdateAvailabilityDTO
            {
                WeekDay = originalAvailability.WeekDay,
                Start = originalAvailability.Start,
                End = originalAvailability.End,

                TeacherId = originalAvailability.TeacherId,
            };

            request.dto.WeekDay = request.dto.WeekDay ?? originalAvailability!.WeekDay;
            request.dto.Start = request.dto.Start ?? originalAvailability.Start;
            request.dto.End = request.dto.End ?? originalAvailability.End;
            request.dto.TeacherId = request.dto.TeacherId ?? originalAvailability?.TeacherId;

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
                    INNER JOIN [Availability] a on a.[TeacherId] = t.[Id]
                    WHERE a.[Id] = @Id;",
                request
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

            var availability = await _dbService.Update<Availability?>(
                $@"
                    UPDATE [Availability] 
                    SET
                    [WeekDay] = @WeekDay, [Start] = @Start, [End] = @End, [TeacherId] = @TeacherId 
                    OUTPUT
                    INSERTED.[Id],
                    INSERTED.[WeekDay],
                    INSERTED.[Start],
                    INSERTED.[End],
                    INSERTED.[TeacherId]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            availability!.Teacher = teacher.Value;

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = request.Id,
                AffectedEntity = "Availability",
                Command = "Patch",
                ReversaleDTO = JsonSerializer.Serialize(reversaleDTOAvailability),
                CollisionChecked = false,
                ScheduleId = teacher.Value.ScheduleId,
            };
            AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
            await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);

            return availability;
        }
    }
}
