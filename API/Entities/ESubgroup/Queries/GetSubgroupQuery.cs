using AlpimiAPI.Database;
using AlpimiAPI.Entities.EGroup;
using AlpimiAPI.Entities.EGroup.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ESubgroup.Queries
{
    public record GetSubgroupQuery(Guid Id, Guid FilteredId, string Role) : IRequest<Subgroup?>;

    public class GetSubgroupHandler : IRequestHandler<GetSubgroupQuery, Subgroup?>
    {
        private readonly IDbService _dbService;

        public GetSubgroupHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<Subgroup?> Handle(
            GetSubgroupQuery request,
            CancellationToken cancellationToken
        )
        {
            Subgroup? subgroup;
            switch (request.Role)
            {
                case "Admin":
                    subgroup = await _dbService.Get<Subgroup?>(
                        @"
                            SELECT 
                            [Id], [Name], [StudentCount], [GroupId] 
                            FROM [Subgroup] 
                            WHERE [Id] = @Id; ",
                        request
                    );
                    break;
                default:
                    subgroup = await _dbService.Get<Subgroup?>(
                        @"
                            SELECT 
                            sg.[Id], sg.[Name], sg.[StudentCount], [GroupId] 
                            FROM [Subgroup] sg
                            INNER JOIN [Group] g ON g.[Id] = sg.[GroupId]
                            INNER JOIN [Schedule] s ON g.[ScheduleId] = s.[Id]
                            WHERE sg.[Id] = @Id AND s.[UserId] = @FilteredId; ",
                        request
                    );
                    break;
            }

            if (subgroup != null)
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
            return subgroup;
        }
    }
}
