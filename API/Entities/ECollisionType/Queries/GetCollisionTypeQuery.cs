using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ECollisionType.Queries
{
    public record GetCollisionTypeQuery(Guid Id, Guid FilteredId, string Role)
        : IRequest<CollisionType?>;

    public class GetCollisionTypeHandler : IRequestHandler<GetCollisionTypeQuery, CollisionType?>
    {
        private readonly IDbService _dbService;

        public GetCollisionTypeHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<CollisionType?> Handle(
            GetCollisionTypeQuery request,
            CancellationToken cancellationToken
        )
        {
            CollisionType? collisionType;
            switch (request.Role)
            {
                case "Admin":
                    collisionType = await _dbService.Get<CollisionType?>(
                        @"
                            SELECT 
                            [Id], [Name], [Description], [Weight], [Filter], [Category], [ScheduleId]
                            FROM [CollisionType] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    collisionType = await _dbService.Get<CollisionType?>(
                        @"
                            SELECT 
                            ct.[Id], ct.[Name], [Description], [Weight], [Filter], [Category], ct.[ScheduleId]
                            FROM [CollisionType] ct
                            INNER JOIN [Schedule] s ON ct.[ScheduleId] = s.[Id]
                            WHERE ct.[Id] = @Id AND s.[UserId] = @FilteredId;",
                        request
                    );
                    break;
            }

            if (collisionType != null)
            {
                GetScheduleHandler getScheduleHandler = new GetScheduleHandler(_dbService);
                GetScheduleQuery getScheduleQuery = new GetScheduleQuery(
                    collisionType.ScheduleId,
                    new Guid(),
                    "Admin"
                );
                ActionResult<Schedule?> schedule = await getScheduleHandler.Handle(
                    getScheduleQuery,
                    cancellationToken
                );
                collisionType.Schedule = schedule.Value!;
            }

            return collisionType;
        }
    }
}
