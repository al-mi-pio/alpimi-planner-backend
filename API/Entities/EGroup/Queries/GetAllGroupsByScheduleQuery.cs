using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EGroup.Queries
{
    public record GetAllGroupsByScheduleQuery(
        Guid ScheduleId,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<Group>?, int)>;

    public class GetAllGroupsByScheduleHandler
        : IRequestHandler<GetAllGroupsByScheduleQuery, (IEnumerable<Group>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public GetAllGroupsByScheduleHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<(IEnumerable<Group>?, int)> Handle(
            GetAllGroupsByScheduleQuery request,
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
                && request.Pagination.SortBy != "StudentCount"
            )
            {
                errors.Add(new ErrorObject(_str["badParameter", "SortBy"]));
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            IEnumerable<Group>? groups;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [Group] 
                            WHERE [ScheduleId] = @ScheduleId;",
                        request
                    );
                    groups = await _dbService.GetAll<Group>(
                        $@"
                            SELECT
                            [Id], [Name], [StudentCount], [ScheduleId] 
                            FROM [Group]
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
                            FROM [Group] g
                            INNER JOIN [Schedule] s ON s.[Id]=g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND g.[ScheduleId] = @ScheduleId;",
                        request
                    );
                    groups = await _dbService.GetAll<Group>(
                        $@"
                            SELECT 
                            g.[Id], g.[Name], [StudentCount],[ScheduleId] 
                            FROM [Group] g
                            INNER JOIN [Schedule] s ON s.[Id]=g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND g.[ScheduleId] = @ScheduleId 
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
            if (groups != null)
            {
                foreach (var group in groups)
                {
                    GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
                    GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                        group.ScheduleId,
                        new Guid(),
                        "Admin"
                    );
                    ActionResult<Schedule?> schedule = await getScheduleHandler.Handle(
                        getScheduleQuery,
                        cancellationToken
                    );
                    group.Schedule = schedule.Value!;
                }
            }
            return (groups, count);
        }
    }
}
