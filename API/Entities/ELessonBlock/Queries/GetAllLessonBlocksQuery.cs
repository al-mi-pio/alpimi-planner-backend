using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.EClassroom.Queries;
using AlpimiAPI.Entities.ELesson;
using AlpimiAPI.Entities.ELesson.Queries;
using AlpimiAPI.Entities.EScheduleSettings;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.ETeacher.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ELessonBlock.Queries
{
    public record GetAllLessonBlocksQuery(
        Guid Id,
        DateOnly? FromDate,
        DateOnly? ToDate,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<LessonBlock>?, int)>;

    public class GetAllLessonBlocksHandler
        : IRequestHandler<GetAllLessonBlocksQuery, (IEnumerable<LessonBlock>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public GetAllLessonBlocksHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<(IEnumerable<LessonBlock>?, int)> Handle(
            GetAllLessonBlocksQuery request,
            CancellationToken cancellationToken
        )
        {
            List<ErrorObject> errors = new List<ErrorObject>();
            if (request.Pagination.PerPage < 0)
            {
                errors.Add(new ErrorObject(_str["badParameter", "PerPage"]));
            }
            if (request.Pagination.Offset < 0)
            {
                errors.Add(new ErrorObject(_str["badParameter", "Page"]));
            }
            if (
                request.Pagination.SortOrder.ToLower() != "asc"
                && request.Pagination.SortOrder.ToLower() != "desc"
            )
            {
                errors.Add(new ErrorObject(_str["badParameter", "SortOrder"]));
            }
            if (
                request.Pagination.SortBy != "Id"
                && request.Pagination.SortBy != "LessonDate"
                && request.Pagination.SortBy != "LessonStart"
                && request.Pagination.SortBy != "LessonEnd"
            )
            {
                errors.Add(new ErrorObject(_str["badParameter", "SortBy"]));
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            var scheduleSettings = await _dbService.Get<ScheduleSettings?>(
                @"
                    SELECT DISTINCT
                    ss.[Id], ss.[SchoolHour], ss.[SchoolYearStart], ss.[SchoolYearEnd], ss.[SchoolDays], ss.[ScheduleId]
                    FROM [ScheduleSettings] ss
                    INNER JOIN [LessonType] lt ON lt.[ScheduleId] = ss.[ScheduleId]
                    INNER JOIN [Lesson] l ON l.[LessonTypeId] = lt.[Id]
                    INNER JOIN [LessonBlock] lb ON lb.[LessonId] = l.[Id]
                    INNER JOIN [Subgroup] sg ON sg.[Id] = l.[SubgroupId]
                    INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                    WHERE sg.[Id] = @Id OR g.[Id] = @Id OR g.[ScheduleId] = @Id OR l.[Id] = @Id OR lb.[TeacherId] = @Id OR lb.[ClassroomId] = @Id OR lb.[ClusterId] = @Id;",
                request
            );
            if (scheduleSettings == null)
            {
                return ([], 0);
            }

            request = request with
            {
                Id = request.Id,
                FromDate = request.FromDate ?? scheduleSettings.SchoolYearStart,
                ToDate = request.ToDate ?? scheduleSettings.SchoolYearEnd
            };

            if (request.FromDate > request.ToDate)
            {
                throw new ApiErrorException([new ErrorObject(_str["scheduleDate"])]);
            }

            IEnumerable<LessonBlock>? lessonBlocks;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [LessonBlock] lb
                            INNER JOIN [Lesson] l ON l.[Id] = lb.[LessonId]
                            INNER JOIN [Subgroup] sg ON sg.[Id] = l.[SubgroupId]
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            WHERE (sg.[Id] = @Id OR g.[Id] = @Id OR g.[ScheduleId] = @Id OR l.[Id] = @Id OR [TeacherId] = @Id OR [ClassroomId] = @Id OR [ClusterId] = @Id) AND lb.[LessonDate] BETWEEN @FromDate AND @ToDate;",
                        request
                    );
                    lessonBlocks = await _dbService.GetAll<LessonBlock>(
                        $@"
                            SELECT
                            lb.[Id], [LessonDate], [LessonStart], [LessonEnd], [LessonId], [ClassroomId], [TeacherId], [ClusterId]  
                            FROM [LessonBlock] lb
                            INNER JOIN [Lesson] l ON l.[Id] = lb.[LessonId]
                            INNER JOIN [Subgroup] sg ON sg.[Id] = l.[SubgroupId]
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            WHERE (sg.[Id] = @Id OR g.[Id] = @Id OR g.[ScheduleId] = @Id OR l.[Id] = @Id OR [TeacherId] = @Id OR [ClassroomId] = @Id OR [ClusterId] = @Id) AND lb.[LessonDate] BETWEEN @FromDate AND @ToDate
                            ORDER BY
                            {request.Pagination.SortBy}
                            {request.Pagination.SortOrder}
                            OFFSET
                            {request.Pagination.Offset} ROWS
                            FETCH NEXT
                            {request.Pagination.PerPage} ROWS ONLY; ",
                        request
                    );
                    break;
                default:
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [LessonBlock] lb
                            INNER JOIN [Lesson] l ON l.[Id] = lb.[LessonId]
                            INNER JOIN [Subgroup] sg ON sg.[Id] = l.[SubgroupId]
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            INNER JOIN [Schedule] s on s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND (sg.[Id] = @Id OR g.[Id] = @Id OR g.[ScheduleId] = @Id OR l.[Id] = @Id OR [TeacherId] = @Id OR [ClassroomId] = @Id OR [ClusterId] = @Id) AND lb.[LessonDate] BETWEEN @FromDate AND @ToDate; ",
                        request
                    );
                    lessonBlocks = await _dbService.GetAll<LessonBlock>(
                        $@"
                            SELECT 
                            lb.[Id], [LessonDate], [LessonStart], [LessonEnd], [LessonId], [ClassroomId], [TeacherId], [ClusterId]   
                            FROM [LessonBlock] lb
                            INNER JOIN [Lesson] l ON l.[Id] = lb.[LessonId]
                            INNER JOIN [Subgroup] sg ON sg.[Id] = l.[SubgroupId]
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            INNER JOIN [Schedule] s on s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND (sg.[Id] = @Id OR g.[Id] = @Id OR g.[ScheduleId] = @Id OR l.[Id] = @Id OR [TeacherId] = @Id OR [ClassroomId] = @Id OR [ClusterId] = @Id) AND lb.[LessonDate] BETWEEN @FromDate AND @ToDate
                            ORDER BY
                            {request.Pagination.SortBy}
                            {request.Pagination.SortOrder}
                            OFFSET
                            {request.Pagination.Offset} ROWS
                            FETCH NEXT
                            {request.Pagination.PerPage} ROWS ONLY; ",
                        request
                    );
                    break;
            }

            if (lessonBlocks != null)
            {
                foreach (var lessonBlock in lessonBlocks)
                {
                    GetLessonHandler getLessonHandler = new GetLessonHandler(_dbService);
                    GetLessonQuery getLessonQuery = new GetLessonQuery(
                        lessonBlock.LessonId,
                        new Guid(),
                        "Admin"
                    );
                    ActionResult<Lesson?> lesson = await getLessonHandler.Handle(
                        getLessonQuery,
                        cancellationToken
                    );
                    lessonBlock.Lesson = lesson.Value!;

                    if (lessonBlock.ClassroomId != null)
                    {
                        GetClassroomHandler getClassroomHandler = new GetClassroomHandler(
                            _dbService
                        );
                        GetClassroomQuery getClassroomQuery = new GetClassroomQuery(
                            lessonBlock.ClassroomId.Value,
                            new Guid(),
                            "Admin"
                        );
                        ActionResult<Classroom?> classroom = await getClassroomHandler.Handle(
                            getClassroomQuery,
                            cancellationToken
                        );
                        lessonBlock.Classroom = classroom.Value!;
                    }

                    if (lessonBlock.TeacherId != null)
                    {
                        GetTeacherHandler getTeacherHandler = new GetTeacherHandler(_dbService);
                        GetTeacherQuery getTeacherQuery = new GetTeacherQuery(
                            lessonBlock.TeacherId.Value,
                            new Guid(),
                            "Admin"
                        );
                        ActionResult<Teacher?> teacher = await getTeacherHandler.Handle(
                            getTeacherQuery,
                            cancellationToken
                        );
                        lessonBlock.Teacher = teacher.Value!;
                    }
                }
            }
            return (lessonBlocks, count);
        }
    }
}
