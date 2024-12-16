using AlpimiAPI.Database;
using AlpimiAPI.Entities.ELessonType;
using AlpimiAPI.Entities.ELessonType.Queries;
using AlpimiAPI.Entities.ESubgroup;
using AlpimiAPI.Entities.ESubgroup.Queries;
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
                            INNER JOIN [Subgroup] sg ON sg.[Id] = l.[SubgroupId]
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            WHERE sg.[Id] = @Id OR g.[Id] = @Id; ",
                        request
                    );
                    lessons = await _dbService.GetAll<Lesson>(
                        $@"
                            SELECT
                            l.[Id], l.[Name], [CurrentHours], [AmountOfHours], l.[LessonTypeId], l.[SubgroupId]  
                            FROM [Lesson] l
                            INNER JOIN [Subgroup] sg ON sg.[Id] = l.[SubgroupId]
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            WHERE sg.[Id] = @Id OR g.[Id] = @Id
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
                            FROM [Lesson] l
                            INNER JOIN [Subgroup] sg ON sg.[Id] = l.[SubgroupId]
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND (sg.[Id] = @Id OR g.[Id] = @Id); ",
                        request
                    );
                    lessons = await _dbService.GetAll<Lesson>(
                        $@"
                            SELECT 
                            l.[Id], l.[Name], [CurrentHours], [AmountOfHours], l.[LessonTypeId], l.[SubgroupId]  
                            FROM [Lesson] l
                            INNER JOIN [Subgroup] sg ON sg.[Id] = l.[SubgroupId]
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND (sg.[Id] = @Id OR g.[Id] = @Id)
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
            if (lessons != null)
            {
                foreach (var lesson in lessons)
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
                    lesson.LessonType = lessonType.Value!;

                    GetSubgroupHandler getSubgroupHandler = new GetSubgroupHandler(_dbService);
                    GetSubgroupQuery getSubgroupQuery = new GetSubgroupQuery(
                        lesson.SubgroupId,
                        new Guid(),
                        "Admin"
                    );
                    ActionResult<Subgroup?> subgroup = await getSubgroupHandler.Handle(
                        getSubgroupQuery,
                        cancellationToken
                    );
                    lesson.Subgroup = subgroup.Value!;
                }
            }
            return (lessons, count);
        }
    }
}
