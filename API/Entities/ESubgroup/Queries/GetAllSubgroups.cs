using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ESubgroup.Queries
{
    public record GetAllSubgroups(
        Guid Id,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<Subgroup>?, int)>;

    public class GetAllSubgroupsHandler
        : IRequestHandler<GetAllSubgroups, (IEnumerable<Subgroup>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public GetAllSubgroupsHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<(IEnumerable<Subgroup>?, int)> Handle(
            GetAllSubgroups request,
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
                            FROM [Subgroup] sg
                            LEFT JOIN [StudentSubgroup] ssg ON ssg.[SubgroupId] = sg.[Id]
                            LEFT JOIN [Student] st ON st.[Id] = ssg.[StudentId]
                            WHERE sg.[GroupId] = @Id OR st.[Id] = @Id",
                        request
                    );
                    subgroups = await _dbService.GetAll<Subgroup>(
                        $@"
                            SELECT
                            sg.[Id], sg.[Name], sg.[StudentCount], sg.[GroupId] 
                            FROM [Subgroup] sg
                            LEFT JOIN [StudentSubgroup] ssg ON ssg.[SubgroupId] = sg.[Id]
                            LEFT JOIN [Student] st ON st.[Id] = ssg.[StudentId]
                            WHERE sg.[GroupId] = @Id OR st.[Id] = @Id
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
                            LEFT JOIN [StudentSubgroup] ssg ON ssg.[SubgroupId] = sg.[Id]
                            LEFT JOIN [Student] st ON st.[Id] = ssg.[StudentId]
                            WHERE s.[UserId] = @FilteredId AND (sg.[GroupId] = @Id OR st.[Id] = @Id)
                            ",
                        request
                    );
                    subgroups = await _dbService.GetAll<Subgroup>(
                        $@"
                            SELECT 
                            sg.[Id], sg.[Name], sg.[StudentCount],sg.[GroupId] 
                            FROM [Subgroup] sg
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            INNER JOIN [Schedule] s ON s.[Id] = g.[ScheduleId]
                            LEFT JOIN [StudentSubgroup] ssg ON ssg.[SubgroupId] = sg.[Id]
                            LEFT JOIN [Student] st ON st.[Id] = ssg.[StudentId]
                            WHERE s.[UserId] = @FilteredId AND (sg.[GroupId] = @Id OR st.[Id] = @Id)
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
