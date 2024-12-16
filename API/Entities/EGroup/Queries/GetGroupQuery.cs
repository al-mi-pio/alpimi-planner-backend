using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.EGroup.Queries
{
    public record GetGroupQuery(Guid Id, Guid FilteredId, string Role) : IRequest<Group?>;

    public class GetGroupHandler : IRequestHandler<GetGroupQuery, Group?>
    {
        private readonly IDbService _dbService;

        public GetGroupHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<Group?> Handle(GetGroupQuery request, CancellationToken cancellationToken)
        {
            Group? group;
            switch (request.Role)
            {
                case "Admin":
                    group = await _dbService.Get<Group?>(
                        @"
                            SELECT 
                            [Id], [Name], [StudentCount], [ScheduleId] 
                            FROM [Group] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    group = await _dbService.Get<Group?>(
                        @"
                            SELECT 
                            g.[Id], g.[Name], [StudentCount], [ScheduleId] 
                            FROM [Group] g
                            INNER JOIN [Schedule] s ON g.[ScheduleId]=s.[Id]
                            WHERE g.[Id] = @Id AND s.[UserId] = @FilteredId;",
                        request
                    );
                    break;
            }

            if (group != null)
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
            return group;
        }
    }
}
