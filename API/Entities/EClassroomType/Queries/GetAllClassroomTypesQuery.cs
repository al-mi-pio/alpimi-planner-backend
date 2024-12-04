using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EClassroomType.Queries
{
    public record GetAllClassroomTypesQuery(
        Guid ScheduleId,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<ClassroomType>?, int)>;

    public class GetAllClassroomTypesHandler
        : IRequestHandler<GetAllClassroomTypesQuery, (IEnumerable<ClassroomType>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public GetAllClassroomTypesHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<(IEnumerable<ClassroomType>?, int)> Handle(
            GetAllClassroomTypesQuery request,
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
            if (request.Pagination.SortBy != "Id" && request.Pagination.SortBy != "Name")
            {
                errors.Add(new ErrorObject(_str["badParameter", "SortBy"]));
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            IEnumerable<ClassroomType>? teachers;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [ClassroomType] 
                            WHERE [ScheduleId] = @ScheduleId",
                        request
                    );
                    teachers = await _dbService.GetAll<ClassroomType>(
                        $@"
                            SELECT
                            [Id], [Name], [ScheduleId] 
                            FROM [ClassroomType]
                            WHERE [ScheduleId] = @ScheduleId 
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
                            SELECT COUNT(*)
                            FROM [ClassroomType] t
                            INNER JOIN [Schedule] s ON s.[Id]=t.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND t.[ScheduleId] = @ScheduleId
                            ",
                        request
                    );
                    teachers = await _dbService.GetAll<ClassroomType>(
                        $@"
                            SELECT 
                            ct.[Id], ct.[Name], [ScheduleId] 
                            FROM [ClassroomType] ct
                            INNER JOIN [Schedule] s ON s.[Id] = ct.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND ct.[ScheduleId] = @ScheduleId 
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
            if (teachers != null)
            {
                foreach (var teacher in teachers)
                {
                    GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
                    GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                        teacher.ScheduleId,
                        new Guid(),
                        "Admin"
                    );
                    ActionResult<Schedule?> schedule = await getScheduleHandler.Handle(
                        getScheduleQuery,
                        cancellationToken
                    );
                    teacher.Schedule = schedule.Value!;
                }
            }
            return (teachers, count);
        }
    }
}
