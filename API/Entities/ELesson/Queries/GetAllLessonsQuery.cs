using AlpimiAPI.Database;
using AlpimiAPI.Entities.EClassroom;
using AlpimiAPI.Entities.ELessonBlock;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Entities.ELessonType.Queries;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.ETeacher;
using AlpimiAPI.Entities.ETeacher.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ELesson.Queries
{
    public record GetAllLessonsQuery(
        Guid Id,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<Lesson>?, int)>;

    public class GetAllLessonsHandler
        : IRequestHandler<GetAllLessonsQuery, (IEnumerable<Lesson>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public GetAllLessonsHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<(IEnumerable<Lesson>?, int)> Handle(
            GetAllLessonsQuery request,
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
                && request.Pagination.SortBy != "Name"
                && request.Pagination.SortBy != "AmountOfHours"
            )
            {
                errors.Add(new ErrorObject(_str["badParameter", "SortBy"]));
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            IEnumerable<Lesson>? lessons;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [Lesson] l
                            INNER JOIN [Teacher] sg ON sg.[Id] = l.[TeacherId]
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            WHERE sg.[Id] = @Id OR g.[Id] = @Id;",
                        request
                    );
                    lessons = await _dbService.GetAll<Lesson>(
                        $@"
                            SELECT
                            l.[Id], l.[Name], [CurrentHours], [AmountOfHours], l.[LessonTypeId], l.[TeacherId]  
                            FROM [Lesson] l
                            INNER JOIN [Teacher] sg ON sg.[Id] = l.[TeacherId]
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            WHERE sg.[Id] = @Id OR g.[Id] = @Id
                            ORDER BY
                            {request.Pagination.SortBy}
                            {request.Pagination.SortOrder}
                            OFFSET
                            {request.Pagination.Offset} ROWS
                            FETCH NEXT
                            {request.Pagination.PerPage} ROWS ONLY;",
                        request
                    );
                    break;
                default:
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [Lesson] l
                            INNER JOIN [Teacher] sg ON sg.[Id] = l.[TeacherId]
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND (sg.[Id] = @Id OR g.[Id] = @Id);",
                        request
                    );
                    lessons = await _dbService.GetAll<Lesson>(
                        $@"
                            SELECT 
                            l.[Id], l.[Name], [CurrentHours], [AmountOfHours], l.[LessonTypeId], l.[TeacherId]  
                            FROM [Lesson] l
                            INNER JOIN [Teacher] sg ON sg.[Id] = l.[TeacherId]
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND (sg.[Id] = @Id OR g.[Id] = @Id)
                            ORDER BY
                            {request.Pagination.SortBy}
                            {request.Pagination.SortOrder}
                            OFFSET
                            {request.Pagination.Offset} ROWS
                            FETCH NEXT
                            {request.Pagination.PerPage} ROWS ONLY;",
                        request
                    );
                    break;
            }

            if (lessons != null)
            {
                Dictionary<Guid, Teacher> subgroupMap = new Dictionary<Guid, Teacher>();
                Dictionary<Guid, LessonType> lessonTypeMap = new Dictionary<Guid, LessonType>();
                foreach (var lesson in lessons)
                {
                    if (!lessonTypeMap.ContainsKey(lesson.LessonTypeId))
                    {
                        GetLessonTypeHandler getLessonTypeHandler = new GetLessonTypeHandler(
                            _dbService
                        );
                        GetLessonTypeQuery getLessonTypeQuery = new GetLessonTypeQuery(
                            lesson.LessonTypeId,
                            new Guid(),
                            "Admin"
                        );
                        ActionResult<LessonType?> lessonType = await getLessonTypeHandler.Handle(
                            getLessonTypeQuery,
                            cancellationToken
                        );

                        lessonTypeMap.Add(lesson.LessonTypeId, lessonType.Value!);
                    }
                    lesson.LessonType = lessonTypeMap[lesson.LessonTypeId];

                    if (!subgroupMap.ContainsKey(lesson.TeacherId))
                    {
                        GetTeacherHandler getTeacherHandler = new GetTeacherHandler(_dbService);
                        GetTeacherQuery getTeacherQuery = new GetTeacherQuery(
                            lesson.TeacherId,
                            new Guid(),
                            "Admin"
                        );
                        ActionResult<Teacher?> subgroup = await getTeacherHandler.Handle(
                            getTeacherQuery,
                            cancellationToken
                        );

                        subgroupMap.Add(lesson.TeacherId, subgroup.Value!);
                    }
                    lesson.Teacher = subgroupMap[lesson.TeacherId];
                }
            }

            return (lessons, count);
        }
    }
}
