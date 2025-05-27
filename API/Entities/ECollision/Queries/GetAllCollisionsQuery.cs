using AlpimiAPI.Database;
using AlpimiAPI.Entities.ECollisionType;
using AlpimiAPI.Entities.ECollisionType.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ECollision.Queries
{
    public record GetAllCollisionsQuery(
        Guid Id,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<Collision>?, int)>;

    public class GetAllCollisionsHandler
        : IRequestHandler<GetAllCollisionsQuery, (IEnumerable<Collision>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public GetAllCollisionsHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<(IEnumerable<Collision>?, int)> Handle(
            GetAllCollisionsQuery request,
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

            IEnumerable<Collision>? collisions;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [Collision] c
                            INNER JOIN [CollisionType] ct ON ct.[Id] = c.[CollisionTypeId]
                            WHERE c.[CollisionTypeId] = @Id OR ct.[ScheduleId] = @Id;",
                        request
                    );
                    collisions = await _dbService.GetAll<Collision>(
                        $@"
                            SELECT
                            c.[Id], [CollidingObject1], [CollidingObject2], [Ignored], c.[CollisionTypeId]
                            FROM [Collision] c
                            INNER JOIN [CollisionType] ct ON ct.[Id] = c.[CollisionTypeId]
                            WHERE c.[CollisionTypeId] = @Id OR ct.[ScheduleId] = @Id
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
                            FROM [Collision] c
                            INNER JOIN [CollisionType] ct ON ct.[Id] = c.[CollisionTypeId]
                            INNER JOIN [Schedule] s ON s.[Id] = ct.[ScheduleId]
                            INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                            WHERE s.[UserId] = @FilteredId AND (c.[CollisionTypeId] = @Id OR ct.[ScheduleId] = @Id);",
                        request
                    );
                    collisions = await _dbService.GetAll<Collision>(
                        $@"
                            SELECT 
                            c.[Id], [CollidingObject1], [CollidingObject2], [Ignored], c.[CollisionTypeId]
                            FROM [Collision] c
                            INNER JOIN [CollisionType] ct ON ct.[Id] = c.[CollisionTypeId]
                            INNER JOIN [Schedule] s ON s.[Id] = ct.[ScheduleId]
                            INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                            WHERE s.[UserId] = @FilteredId AND (c.[CollisionTypeId] = @Id OR ct.[ScheduleId] = @Id)
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

            if (collisions != null)
            {
                Dictionary<Guid, CollisionType> collisionTypeMap =
                    new Dictionary<Guid, CollisionType>();
                foreach (var collision in collisions)
                {
                    if (!collisionTypeMap.ContainsKey(collision.CollisionTypeId))
                    {
                        GetCollisionTypeHandler getCollisionTypeHandler =
                            new GetCollisionTypeHandler(_dbService);
                        GetCollisionTypeQuery getCollisionTypeQuery = new GetCollisionTypeQuery(
                            collision.CollisionTypeId,
                            new Guid(),
                            "Admin"
                        );
                        ActionResult<CollisionType?> collisionType =
                            await getCollisionTypeHandler.Handle(
                                getCollisionTypeQuery,
                                cancellationToken
                            );

                        collisionTypeMap.Add(collision.CollisionTypeId, collisionType.Value!);
                    }
                    collision.CollisionType = collisionTypeMap[collision.CollisionTypeId];
                }
            }

            return (collisions, count);
        }
    }
}
