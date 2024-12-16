using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.EClassroom.Queries;
using AlpimiAPI.Entities.EClassroomType;
using AlpimiAPI.Entities.EClassroomType.Queries;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Queries;
using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ELessonBlock.DTO;
using AlpimiAPI.Entities.ELessonBlock.Queries;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.Queries;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.ETeacher.Queries;
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

        public UpdateLessonBlockHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid?> Handle(
            UpdateLessonBlockCommand request,
            CancellationToken cancellationToken
        )
        {
            LessonBlock oneLessonBlock;
            LessonBlock lastLessonBlock;
            int amountOfLessonsToUpdate = 1;
            int amountOfLessonHoursToUpdate;

            if (request.dto.UpdateCluster)
            {
                GetAllLessonBlocksHandler getAllLessonBlocksHandler = new GetAllLessonBlocksHandler(
                    _dbService,
                    _str
                );
                GetAllLessonBlocksQuery getAllLessonBlocksQuery = new GetAllLessonBlocksQuery(
                    request.Id,
                    null,
                    null,
                    request.FilteredId,
                    request.Role,
                    new PaginationParams(int.MaxValue, 0, "LessonDate", "ASC")
                );
                ActionResult<(IEnumerable<LessonBlock>?, int)> lessonBlocks =
                    await getAllLessonBlocksHandler.Handle(
                        getAllLessonBlocksQuery,
                        cancellationToken
                    );

                if (lessonBlocks.Value.Item2 == 0)
                {
                    return null;
                }
                oneLessonBlock = lessonBlocks.Value.Item1!.First();
                lastLessonBlock = lessonBlocks.Value.Item1!.Last();
                amountOfLessonsToUpdate = lessonBlocks.Value.Item2;
                amountOfLessonHoursToUpdate =
                    (
                        (
                            request.dto.LessonEnd
                            ?? oneLessonBlock.LessonEnd - request.dto.LessonStart
                            ?? oneLessonBlock.LessonStart + 1
                        ) - (oneLessonBlock.LessonEnd - oneLessonBlock.LessonStart + 1)
                    ) * lessonBlocks.Value.Item2;
            }
            else
            {
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

                if (lessonBlock.Value == null)
                {
                    return null;
                }

                oneLessonBlock = lessonBlock.Value;
                lastLessonBlock = oneLessonBlock;
                amountOfLessonHoursToUpdate =
                    (
                        request.dto.LessonEnd
                        ?? oneLessonBlock.LessonEnd - request.dto.LessonStart
                        ?? oneLessonBlock.LessonStart + 1
                    ) - (oneLessonBlock.LessonEnd - oneLessonBlock.LessonStart + 1);
            }

            request.dto.LessonEnd = request.dto.LessonEnd ?? oneLessonBlock.LessonEnd;
            request.dto.LessonStart = request.dto.LessonStart ?? oneLessonBlock.LessonStart;
            request.dto.ClassroomId = request.dto.ClassroomId ?? oneLessonBlock.ClassroomId;
            request.dto.TeacherId = request.dto.TeacherId ?? oneLessonBlock.TeacherId;
            request.dto.WeekDay = request.dto.WeekDay ?? (int)oneLessonBlock.LessonDate.DayOfWeek;

            List<ErrorObject> errors = new List<ErrorObject>();
            if (request.dto.TeacherId != null)
            {
                GetTeacherHandler getTeacherHandler = new GetTeacherHandler(_dbService);
                GetTeacherQuery getTeacherQuery = new GetTeacherQuery(
                    request.dto.TeacherId.Value,
                    request.FilteredId,
                    request.Role
                );
                ActionResult<Teacher?> teacher = await getTeacherHandler.Handle(
                    getTeacherQuery,
                    cancellationToken
                );
                if (teacher.Value == null)
                {
                    errors.Add(
                        new ErrorObject(_str["resourceNotFound", "Teacher", request.dto.TeacherId])
                    );
                }
                else if (oneLessonBlock.Lesson.LessonType.ScheduleId != teacher.Value.ScheduleId)
                {
                    errors.Add(new ErrorObject(_str["wrongSet", "Teacher", "Schedule", "Lesson"]));
                }
            }

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
                            _str["resourceNotFound", "Classroom", request.dto.ClassroomId]
                        )
                    );
                }
                else if (oneLessonBlock.Lesson.LessonType.ScheduleId != classroom.Value.ScheduleId)
                {
                    errors.Add(
                        new ErrorObject(_str["wrongSet", "Classroom", "Schedule", "Lesson"])
                    );
                }
            }

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

            var lessonPeriodCount = await _dbService.Get<int>(
                $@"
                    SELECT 
                    count(*)
                    FROM [LessonPeriod] 
                    WHERE ScheduleSettingsId = '{scheduleSettings!.Id}';",
                ""
            );

            if (request.dto.LessonStart > request.dto.LessonEnd)
            {
                errors.Add(new ErrorObject(_str["scheduleTime"]));
            }

            if (request.dto.LessonStart < 1)
            {
                errors.Add(new ErrorObject(_str["badParameter", "LessonStart"]));
            }

            if (request.dto.LessonEnd > lessonPeriodCount)
            {
                errors.Add(new ErrorObject(_str["badParameter", "LessonEnd"]));
            }

            if (scheduleSettings!.SchoolDays[request.dto.WeekDay.Value] == '0')
            {
                errors.Add(
                    new ErrorObject(_str["badWeekDay", (DayOfWeek)request.dto.WeekDay.Value])
                );
            }

            int daysDifference =
                request.dto.WeekDay.Value - (int)oneLessonBlock.LessonDate.DayOfWeek;
            if (
                scheduleSettings.SchoolYearStart > oneLessonBlock.LessonDate.AddDays(daysDifference)
                || scheduleSettings.SchoolYearEnd
                    < oneLessonBlock.LessonDate.AddDays(daysDifference)
                || scheduleSettings.SchoolYearStart
                    > lastLessonBlock.LessonDate.AddDays(daysDifference)
                || scheduleSettings.SchoolYearEnd
                    < lastLessonBlock.LessonDate.AddDays(daysDifference)
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
                    [LessonDate] = DATEADD(DAY,{daysDifference},[LessonDate]), 
                    [LessonStart] = @LessonStart, 
                    [LessonEnd] = @LessonEnd, 
                    [ClassroomId] = @ClassroomId, 
                    [TeacherId] = @TeacherId
                    WHERE [Id] = '{request.Id}' OR [ClusterId] = '{request.Id}';",
                request.dto
            );

            await Utilities.CurrentLessonHours.Update(
                _dbService,
                oneLessonBlock.LessonId,
                cancellationToken
            );

            return request.Id;
        }
    }
}
