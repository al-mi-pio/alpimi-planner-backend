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
        private readonly IStringLocalizer<Fields> _strFields;

        public GetAllGroupsByScheduleHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<(IEnumerable<Group>?, int)> Handle(
            GetAllGroupsByScheduleQuery request,
            CancellationToken cancellationToken
        )
        {
            List<ErrorObject> errors = new List<ErrorObject>();
            if (request.Pagination.PerPage < 0)
            {
                errors.Add(
                    new FieldErrorObject("PerPage", _str["badParameter", _strFields["PerPage"]])
                );
            }
            if (request.Pagination.Offset < 0)
            {
                errors.Add(new FieldErrorObject("Page", _str["badParameter", _strFields["Page"]]));
            }
            if (
                request.Pagination.SortOrder.ToLower() != "asc"
                && request.Pagination.SortOrder.ToLower() != "desc"
            )
            {
                errors.Add(
                    new FieldErrorObject("SortOrder", _str["badParameter", _strFields["SortOrder"]])
                );
            }
            if (
                request.Pagination.SortBy != "Id"
                && request.Pagination.SortBy != "Name"
                && request.Pagination.SortBy != "StudentCount"
            )
            {
                errors.Add(
                    new FieldErrorObject("SortBy", _str["badParameter", _strFields["SortBy"]])
                );
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
                case "User":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT
                            COUNT(*)
                            FROM [Group] g
                            INNER JOIN [Schedule] s ON s.[Id]=g.[ScheduleId]
                            INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                            WHERE (s.[UserId] = @FilteredId OR ss.[IsPublic] = 'TRUE') AND g.[ScheduleId] = @ScheduleId;",
                        request
                    );
                    groups = await _dbService.GetAll<Group>(
                        $@"
                            SELECT 
                            g.[Id], g.[Name], [StudentCount], [ScheduleId] 
                            FROM [Group] g
                            INNER JOIN [Schedule] s ON s.[Id]=g.[ScheduleId]
                            INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                            WHERE (s.[UserId] = @FilteredId OR ss.[IsPublic] = 'TRUE') AND g.[ScheduleId] = @ScheduleId 
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
                            INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                            WHERE ss.[IsPublic] = 'TRUE' AND g.[ScheduleId] = @ScheduleId;",
                        request
                    );
                    groups = await _dbService.GetAll<Group>(
                        $@"
                            SELECT 
                            g.[Id], g.[Name], [StudentCount], g.[ScheduleId] 
                            FROM [Group] g
                            INNER JOIN [Schedule] s ON s.[Id]=g.[ScheduleId]
                            INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                            WHERE ss.[IsPublic] = 'TRUE' AND g.[ScheduleId] = @ScheduleId 
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
                Dictionary<Guid, Schedule> scheduleMap = new Dictionary<Guid, Schedule>();
                foreach (var group in groups)
                {
                    if (!scheduleMap.ContainsKey(group.ScheduleId))
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

                        scheduleMap.Add(group.ScheduleId, schedule.Value!);
                    }
                    group.Schedule = scheduleMap[group.ScheduleId];
                }
            }

            return (groups, count);
        }
    }
}
