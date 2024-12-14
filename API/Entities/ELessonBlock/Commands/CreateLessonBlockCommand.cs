﻿using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.EClassroom.Queries;
using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ELesson.Queries;
using AlpimiAPI.Entities.ELessonBlock.DTO;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.ETeacher.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ELessonBlock.Commands
{
    public record CreateLessonBlockCommand(
        Guid Id,
        Guid ClusterId,
        CreateLessonBlockDTO dto,
        Guid FilteredId,
        string Role
    ) : IRequest<Guid>;

    public class CreateLessonBlockHandler : IRequestHandler<CreateLessonBlockCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public CreateLessonBlockHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<Guid> Handle(
            CreateLessonBlockCommand request,
            CancellationToken cancellationToken
        )
        {
            List<ErrorObject> errors = new List<ErrorObject>();

            GetLessonHandler getLessonHandler = new GetLessonHandler(_dbService);
            GetLessonQuery getLessonQuery = new GetLessonQuery(
                request.dto.LessonId,
                request.FilteredId,
                request.Role
            );
            ActionResult<Lesson?> lesson = await getLessonHandler.Handle(
                getLessonQuery,
                cancellationToken
            );
            if (lesson.Value == null)
            {
                errors.Add(
                    new ErrorObject(_str["resourceNotFound", "Lesson", request.dto.LessonId])
                );
            }

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
                else if (lesson.Value != null)
                {
                    if (lesson.Value.LessonType.ScheduleId != teacher.Value.ScheduleId)
                    {
                        errors.Add(
                            new ErrorObject(_str["wrongSet", "Teacher", "Schedule", "Lesson"])
                        );
                    }
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
                else if (lesson.Value != null)
                {
                    if (lesson.Value.LessonType.ScheduleId != classroom.Value.ScheduleId)
                    {
                        errors.Add(
                            new ErrorObject(_str["wrongSet", "Classroom", "Schedule", "Lesson"])
                        );
                    }
                }
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            var scheduleSettings = await _dbService.Get<ScheduleSettings?>(
                @"
                    SELECT 
                    ss.[Id], ss.[SchoolHour], ss.[SchoolYearStart], ss.[SchoolYearEnd], ss.[ScheduleId]
                    FROM [LessonBlock] lb
                    INNER JOIN [Lesson] l ON l.[Id] = lb.[LessonId]
                    INNER JOIN [LessonType] lt ON lt.[Id] = l.[LessonTypeId]
                    INNER JOIN [Schedule] s ON s.[Id] = lt.[ScheduleId]
                    INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                    WHERE l.[Id] = @LessonId;",
                request.dto
            );

            var lessonPeriodCount = await _dbService.Get<int>(
                @"
                    SELECT 
                    count(*)
                    FROM [LessonPeriod] 
                    WHERE ScheduleSettingsId = @Id;",
                scheduleSettings!
            );

            if (request.dto.LessonStart < request.dto.LessonEnd)
            {
                errors.Add(new ErrorObject(_str["scheduleTime"]));
            }

            if (request.dto.LessonStart < 0 || request.dto.LessonStart > lessonPeriodCount)
            {
                errors.Add(new ErrorObject(_str["badParameter", "LessonStart"]));
            }

            if (request.dto.LessonEnd < 0 || request.dto.LessonEnd > lessonPeriodCount)
            {
                errors.Add(new ErrorObject(_str["badParameter", "LessonEnd"]));
            }

            if (
                request.dto.LessonDate < scheduleSettings!.SchoolYearStart
                || request.dto.LessonDate > scheduleSettings!.SchoolYearEnd
            )
            {
                errors.Add(
                    new ErrorObject(
                        _str[
                            "dateOutOfRange",
                            scheduleSettings!.SchoolYearStart,
                            scheduleSettings!.SchoolYearEnd
                        ]
                    )
                );
            }

            if (scheduleSettings!.SchoolDays[(int)request.dto.LessonDate.DayOfWeek - 1] == '0')
            {
                errors.Add(new ErrorObject(_str["badWeekDay", request.dto.LessonDate.DayOfWeek]));
            }

            int amountOfLessonsToInsert = 1;
            if (request.dto.WeekInterval != null)
            {
                if (request.dto.WeekInterval < 1)
                {
                    errors.Add(new ErrorObject(_str["badParameter", "WeekInterval"]));
                }
                amountOfLessonsToInsert = Convert.ToInt32(
                    Math.Floor(
                        (
                            scheduleSettings.SchoolYearEnd.DayNumber
                            - request.dto.LessonDate.DayNumber
                        ) / 7.0
                    )
                );
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            int amountOfHoursToAdd =
                amountOfLessonsToInsert * (request.dto.LessonEnd - request.dto.LessonStart + 1);
            await _dbService.Update<Lesson?>(
                $@"
                    UPDATE [Lesson] 
                    SET
                    [CurrentHours] = [CurrentHours] + {amountOfHoursToAdd}
                    OUTPUT
                    INSERTED.[Id]
                    WHERE [Id] = @Id;",
                lesson.Value!
            );

            for (int i = 0; i < amountOfLessonsToInsert; i++)
            {
                await _dbService.Post<Guid>(
                    $@"
                    INSERT INTO [LessonBlock] 
                    ([Id],[LessonDate],[LessonStart],[LessonEnd],[LessonId],[ClassroomId],[TeacherId],[ClusterId])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',   
                    @LessonDate,
                    @LessonStart,
                    @LessonEnd,
                    @LessonId,
                    @ClassroomId,
                    @TeacherId
                    @ClusterId);",
                    request.dto
                );
                request.dto.LessonDate = DateOnly.FromDayNumber(
                    request.dto.LessonDate.DayNumber + 7
                );
                request = request with
                {
                    Id = Guid.NewGuid(),
                    ClusterId = request.ClusterId,
                    dto = request.dto,
                    FilteredId = request.FilteredId,
                    Role = request.Role
                };
            }

            if (amountOfLessonsToInsert > 1)
            {
                return request.ClusterId;
            }

            return request.Id;
        }
    }
}