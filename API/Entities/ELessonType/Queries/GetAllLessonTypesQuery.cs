using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ELessonType.Queries
{
    public record GetAllLessonTypesQuery(
        Guid ScheduleId,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<LessonType>?, int)>;

    public class GetAllLessonTypesHandler
        : IRequestHandler<GetAllLessonTypesQuery, (IEnumerable<LessonType>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public GetAllLessonTypesHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<(IEnumerable<LessonType>?, int)> Handle(
            GetAllLessonTypesQuery request,
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
                && request.Pagination.SortBy != "Capacity"
            )
            {
                errors.Add(new ErrorObject(_str["badParameter", "SortBy"]));
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            IEnumerable<LessonType>? lessonTypes;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [LessonType] 
                            WHERE [ScheduleId] = @ScheduleId;",
                        request
                    );
                    lessonTypes = await _dbService.GetAll<LessonType>(
                        $@"
                            SELECT
                            [Id], [Name], [Color], [ScheduleId] 
                            FROM [LessonType]
                            WHERE [ScheduleId] = @ScheduleId 
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
                            FROM [LessonType] lt
                            INNER JOIN [Schedule] s ON s.[Id] = lt.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND lt.[ScheduleId] = @ScheduleId;",
                        request
                    );
                    lessonTypes = await _dbService.GetAll<LessonType>(
                        $@"
                            SELECT 
                            lt.[Id], lt.[Name], lt.[Color], [ScheduleId] 
                            FROM [LessonType] lt
                            INNER JOIN [Schedule] s ON s.[Id] = lt.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND lt.[ScheduleId] = @ScheduleId 
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

            if (lessonTypes != null)
            {
                foreach (var lessonType in lessonTypes)
                {
                    GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
                    GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                        lessonType.ScheduleId,
                        new Guid(),
                        "Admin"
                    );
                    ActionResult<Schedule?> schedule = await getScheduleHandler.Handle(
                        getScheduleQuery,
                        cancellationToken
                    );
                    lessonType.Schedule = schedule.Value!;
                }
            }

            return (lessonTypes, count);
        }
    }
}
