using AlpimiAPI.Database;
using AlpimiAPI.Entities.ESchedule;
using AlpimiAPI.Entities.ESchedule.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.ECollisionType.Queries
{
    public record GetAllCollisionTypesByScheduleQuery(
        Guid ScheduleId,
        Guid FilteredId,
        string Role,
        PaginationParams Pagination
    ) : IRequest<(IEnumerable<CollisionType>?, int)>;

    public class GetAllCollisionTypesByScheduleHandler
        : IRequestHandler<GetAllCollisionTypesByScheduleQuery, (IEnumerable<CollisionType>?, int)>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public GetAllCollisionTypesByScheduleHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str
        )
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<(IEnumerable<CollisionType>?, int)> Handle(
            GetAllCollisionTypesByScheduleQuery request,
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
                && request.Pagination.SortBy != "Description"
                && request.Pagination.SortBy != "Weight"
                && request.Pagination.SortBy != "Filter"
                && request.Pagination.SortBy != "Category"
            )
            {
                errors.Add(new ErrorObject(_str["badParameter", "SortBy"]));
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            IEnumerable<CollisionType>? collisionTypes;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>(
                        @"
                            SELECT 
                            COUNT(*)
                            FROM [CollisionType] 
                            WHERE [ScheduleId] = @ScheduleId;",
                        request
                    );
                    collisionTypes = await _dbService.GetAll<CollisionType>(
                        $@"
                            SELECT
                            [Id], [Name], [Description], [Weight], [Filter], [Category], [ScheduleId]
                            FROM [CollisionType]
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
                            FROM [CollisionType] ct
                            INNER JOIN [Schedule] s ON s.[Id] = ct.[ScheduleId]
                            INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                            WHERE s.[UserId] = @FilteredId AND ct.[ScheduleId] = @ScheduleId;",
                        request
                    );
                    collisionTypes = await _dbService.GetAll<CollisionType>(
                        $@"
                            SELECT 
                            ct.[Id], ct.[Name], [Description], [Weight], [Filter], [Category], ct.[ScheduleId]
                            FROM [CollisionType] ct
                            INNER JOIN [Schedule] s ON s.[Id] = ct.[ScheduleId]
                            INNER JOIN [ScheduleSettings] ss ON ss.[ScheduleId] = s.[Id]
                            WHERE s.[UserId] = @FilteredId AND ct.[ScheduleId] = @ScheduleId 
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

            if (collisionTypes != null)
            {
                Dictionary<Guid, Schedule> scheduleMap = new Dictionary<Guid, Schedule>();
                foreach (var collisionType in collisionTypes)
                {
                    if (!scheduleMap.ContainsKey(collisionType.ScheduleId))
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

                        scheduleMap.Add(collisionType.ScheduleId, schedule.Value!);
                    }
                    collisionType.Schedule = scheduleMap[collisionType.ScheduleId];
                }
            }

            return (collisionTypes, count);
        }
    }
}
