using System.Text.Json;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.EClassroom.Queries;
using AlpimiAPI.Entities.EDayOff.Commands;
using AlpimiAPI.Entities.EHistory.DTO;
using AlpimiAPI.Entities.ELessonBlock.DTO;
using AlpimiAPI.Entities.ELessonBlock.Queries;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ELessonBlock.Commands
{
    public record UpdateLessonBlockCommand(
        Guid Id,
        UpdateLessonBlockDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Guid?>;

    public class UpdateLessonBlockHandler : IRequestHandler<UpdateLessonBlockCommand, Guid?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public UpdateLessonBlockHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<Guid?> Handle(
            UpdateLessonBlockCommand request,
            CancellationToken cancellationToken
        )
        {
            LessonBlock oneLessonBlock;
            LessonBlock lastLessonBlock;

            GetLessonBlockHandler getLessonBlockHandler = new GetLessonBlockHandler(_dbService);
            GetLessonBlockQuery getLessonBlockQuery = new GetLessonBlockQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<LessonBlock?> lessonBlock = await getLessonBlockHandler.Handle(
                getLessonBlockQuery,
                cancellationToken
            );
            var affectedEntity = lessonBlock.Value != null ? "LessonBlock" : "LessonBlockCluster";

            if (lessonBlock.Value == null)
            {
                GetAllLessonBlocksHandler getFirstLessonBlocksHandler =
                    new GetAllLessonBlocksHandler(_dbService, _str, _strFields);
                GetAllLessonBlocksQuery getFirstLessonBlocksQuery = new GetAllLessonBlocksQuery(
                    request.Id,
                    null,
                    null,
                    request.FilteredId,
                    request.Role,
                    new PaginationParams(1, 0, "LessonDate", "ASC")
                );
                ActionResult<(IEnumerable<LessonBlock>?, int)> firstLessonBlock =
                    await getFirstLessonBlocksHandler.Handle(
                        getFirstLessonBlocksQuery,
                        cancellationToken
                    );

                if (firstLessonBlock.Value.Item2 == 0)
                {
                    return null;
                }

                GetAllLessonBlocksHandler getLastLessonBlocksHandler =
                    new GetAllLessonBlocksHandler(_dbService, _str, _strFields);
                GetAllLessonBlocksQuery getLastLessonBlocksQuery = new GetAllLessonBlocksQuery(
                    request.Id,
                    null,
                    null,
                    request.FilteredId,
                    request.Role,
                    new PaginationParams(1, 0, "LessonDate", "DESC")
                );
                ActionResult<(IEnumerable<LessonBlock>?, int)> finalLessonBlock =
                    await getFirstLessonBlocksHandler.Handle(
                        getFirstLessonBlocksQuery,
                        cancellationToken
                    );

                oneLessonBlock = firstLessonBlock.Value.Item1!.First();
                lastLessonBlock = firstLessonBlock.Value.Item1!.First();
            }
            else
            {
                oneLessonBlock = lessonBlock.Value;
                lastLessonBlock = oneLessonBlock;
            }

            List<ErrorObject> errors = new List<ErrorObject>();
            if (request.dto.ClassroomId != null)
            {
                GetClassroomHandler getClassroomHandler = new GetClassroomHandler(_dbService);
                GetClassroomQuery getClassroomQuery = new GetClassroomQuery(
                    request.dto.ClassroomId.Value,
                    request.FilteredId,
                    request.Role
                );
                ActionResult<Classroom?> classroom = await getClassroomHandler.Handle(
                    getClassroomQuery,
                    cancellationToken
                );
                if (classroom.Value == null)
                {
                    errors.Add(
                        new ErrorObject(
                            _str[
                                "resourceNotFound",
                                _strFields["Classroom"],
                                request.dto.ClassroomId
                            ]
                        )
                    );
                }
                else if (oneLessonBlock.Lesson.LessonType.ScheduleId != classroom.Value.ScheduleId)
                {
                    errors.Add(
                        new ErrorObject(
                            _str[
                                "wrongSet",
                                _strFields["Classroom"],
                                _strFields["Schedule"],
                                _strFields["Lesson"]
                            ]
                        )
                    );
                }
            }

            UpdateLessonBlockDTO reversaleDTOLessonBlock = new UpdateLessonBlockDTO
            {
                LessonEnd = oneLessonBlock.LessonEnd,
                LessonStart = oneLessonBlock.LessonStart,
                ClassroomId = oneLessonBlock.ClassroomId,
                WeekDay = (int)oneLessonBlock.LessonDate.DayOfWeek,
            };

            request.dto.LessonEnd = request.dto.LessonEnd ?? oneLessonBlock.LessonEnd;
            request.dto.LessonStart = request.dto.LessonStart ?? oneLessonBlock.LessonStart;
            request.dto.ClassroomId = request.dto.ClassroomId ?? oneLessonBlock.ClassroomId;
            request.dto.WeekDay = request.dto.WeekDay ?? (int)oneLessonBlock.LessonDate.DayOfWeek;

            var scheduleSettings = await _dbService.Get<ScheduleSettings?>(
                @"
                    SELECT DISTINCT
                    ss.[Id], ss.[SchoolHour], ss.[SchoolYearStart], ss.[SchoolYearEnd], ss.[SchoolDays], ss.[ScheduleId]
                    FROM [LessonBlock] lb
                    INNER JOIN [Lesson] l ON l.[Id] = lb.[LessonId]
                    INNER JOIN [LessonType] lt ON lt.[Id] = l.[LessonTypeId]
                    INNER JOIN [Schedule] s ON s.[Id] = lt.[ScheduleId]
                    INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                    WHERE lb.[Id] = @Id OR lb.[ClusterId] = @Id;",
                request
            );

            var lessonPeriodCount = Utilities
                .LessonPeriodCount.Get(_dbService, scheduleSettings!.Id, cancellationToken)
                .Result;

            if (request.dto.LessonStart > request.dto.LessonEnd)
            {
                errors.Add(new ErrorObject(_str["scheduleTime"]));
            }

            if (request.dto.LessonStart < 0)
            {
                errors.Add(
                    new FieldErrorObject(
                        "lessonStart",
                        _str["badParameter", _strFields["LessonStart"]]
                    )
                );
            }

            if (request.dto.LessonEnd >= lessonPeriodCount)
            {
                errors.Add(
                    new FieldErrorObject("lessonEnd", _str["badParameter", _strFields["LessonEnd"]])
                );
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

            if (scheduleSettings!.SchoolDays[request.dto.WeekDay.Value] == '0')
            {
                errors.Add(
                    new ErrorObject(_str["badWeekDay", (DayOfWeek)request.dto.WeekDay.Value])
                );
            }

            int daysDifferenceFirst =
                request.dto.WeekDay.Value - (int)oneLessonBlock.LessonDate.DayOfWeek;
            int daysDifferenceLast =
                request.dto.WeekDay.Value - (int)lastLessonBlock.LessonDate.DayOfWeek;
            if (
                scheduleSettings.SchoolYearStart
                    > oneLessonBlock.LessonDate.AddDays(daysDifferenceFirst)
                || scheduleSettings.SchoolYearEnd
                    < oneLessonBlock.LessonDate.AddDays(daysDifferenceFirst)
                || scheduleSettings.SchoolYearStart
                    > lastLessonBlock.LessonDate.AddDays(daysDifferenceLast)
                || scheduleSettings.SchoolYearEnd
                    < lastLessonBlock.LessonDate.AddDays(daysDifferenceLast)
            )
            {
                errors.Add(
                    new ErrorObject(
                        _str[
                            "dateOutOfRange",
                            scheduleSettings.SchoolYearStart,
                            scheduleSettings.SchoolYearEnd
                        ]
                    )
                );
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            await _dbService.Update<LessonBlock?>(
                $@"
                    UPDATE [LessonBlock]
                    SET
                    [LessonDate] = DATEADD(DAY,@WeekDay - DATEPART(WEEKDAY, [LessonDate]) + 1,[LessonDate]),
                    [LessonStart] = @LessonStart,
                    [LessonEnd] = @LessonEnd,
                    [ClassroomId] = @ClassroomId
                    WHERE [Id] = '{request.Id}' OR [ClusterId] = '{request.Id}';
                ",
                request.dto
            );

            await Utilities.CurrentLessonHours.Update(
                _dbService,
                oneLessonBlock.LessonId,
                cancellationToken
            );

            AddToHistoryHandler addToHistoryHandler = new AddToHistoryHandler(_dbService);
            AddToHistoryDTO addToHistoryDTO = new AddToHistoryDTO
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AffectedEntityId = request.Id,
                AffectedEntity = affectedEntity,
                Command = "Patch",
                ReversaleDTO = JsonSerializer.Serialize(reversaleDTOLessonBlock),
                CollisionChecked = false,
                ScheduleId = oneLessonBlock.Lesson.LessonType.ScheduleId,
            };
            AddToHistoryCommand addToHistoryCommand = new AddToHistoryCommand(addToHistoryDTO);
            await addToHistoryHandler.Handle(addToHistoryCommand, cancellationToken);

            return request.Id;
        }
    }
}
