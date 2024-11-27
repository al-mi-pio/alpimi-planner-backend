using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Queries;
using AlpimiAPI.Responses;
using alpimi_planner_backend.API.Locales;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ESubgroup.Queries
{
    public record GetAllSubgroupsByGroupQuery(
        Guid GroupId,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<Subgroup>?, int)>;

    public class GetAllSubgroupsByGroupHandler
        : IRequestHandler<GetAllSubgroupsByGroupQuery, (IEnumerable<Subgroup>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public GetAllSubgroupsByGroupHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<(IEnumerable<Subgroup>?, int)> Handle(
            GetAllSubgroupsByGroupQuery request,
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
                request.Pagination.SortBy.ToLower() != "id"
                && request.Pagination.SortBy.ToLower() != "name"
                && request.Pagination.SortBy.ToLower() != "surname"
            )
            {
                errors.Add(new ErrorObject(_str["badParameter", "SortBy"]));
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            IEnumerable<Subgroup>? subgroups;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [Subgroup] 
                            WHERE [GroupId] = @GroupId",
                        request
                    );
                    subgroups = await _dbService.GetAll<Subgroup>(
                        $@"
                            SELECT
                            [Id], [Name], [StudentCount],[GroupId] 
                            FROM [Subgroup]
                            WHERE [GroupId] = @GroupId 
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
                            FROM [Subgroup] sg
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND sg.[GroupId] = @GroupId
                            ",
                        request
                    );
                    subgroups = await _dbService.GetAll<Subgroup>(
                        $@"
                            SELECT 
                            sg.[Id], sg.[Name], sg.[StudentCount],[GroupId] 
                            FROM [Subgroup] sg
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            WHERE s.[UserId] = @FilteredId AND sg.[GroupId] = @GroupId 
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
            if (subgroups != null)
            {
                foreach (var subgroup in subgroups)
                {
                    GetGroupHandler getGroupHandler = new GetGroupHandler(_dbService);
                    GetGroupQuery getGroupQuery = new GetGroupQuery(
                        subgroup.GroupId,
                        new Guid(),
                        "Admin"
                    );
                    ActionResult<Group?> group = await getGroupHandler.Handle(
                        getGroupQuery,
                        cancellationToken
                    );
                    subgroup.Group = group.Value!;
                }
            }
            return (subgroups, count);
        }
    }
}
