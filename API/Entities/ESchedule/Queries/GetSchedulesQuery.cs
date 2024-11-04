using AlpimiAPI.Database;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ESchedule.Queries
{
    public record GetSchedulesQuery(
        Guid FilteredId,
        string Role,
        int PerPage,
        int Offset,
        string SortBy,
        string SortOrder
    ) : IRequest<(IEnumerable<Schedule>?, int)>;

    public class GetSchedulesHandler
        : IRequestHandler<GetSchedulesQuery, (IEnumerable<Schedule>?, int)>
    {
        private readonly IDbService _dbService;

        public GetSchedulesHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<(IEnumerable<Schedule>?, int)> Handle(
            GetSchedulesQuery request,
            CancellationToken cancellationToken
        )
        {
            if (request.PerPage < 0)
            {
                throw new BadHttpRequestException("Bad PerPage");
            }
            if (request.Offset < 0)
            {
                throw new BadHttpRequestException("Bad Page");
            }
            if (request.SortOrder.ToLower() != "asc" && request.SortOrder.ToLower() != "desc")
            {
                throw new BadHttpRequestException("Bad SortOrder");
            }
            if (
                request.SortBy.ToLower() != "id"
                && request.SortBy.ToLower() != "name"
                && request.SortBy.ToLower() != "schoolhour"
                && request.SortBy.ToLower() != "userid"
            )
            {
                throw new BadHttpRequestException("Bad SortBy");
            }
            IEnumerable<Schedule>? schedules;
            int count;
            switch (request.Role)
            {
                case "Admin":
                    count = await _dbService.Get<int>("SELECT COUNT(*) from [Schedule]", request);
                    schedules = await _dbService.GetAll<Schedule>(
                        "SELECT [Id], [Name], [SchoolHour], [UserId] FROM [Schedule] ORDER BY '"
                            + request.SortBy
                            + "' "
                            + request.SortOrder
                            + " OFFSET @Offset ROWS FETCH NEXT @PerPage ROWS ONLY;",
                        request
                    );
                    break;
                default:
                    count = await _dbService.Get<int>(
                        "SELECT COUNT(*) from [Schedule] WHERE [UserId] = @FilteredId",
                        request
                    );
                    schedules = await _dbService.GetAll<Schedule>(
                        "SELECT [Id], [Name], [SchoolHour], [UserId] FROM [Schedule] WHERE [UserId] = @FilteredId ORDER BY'"
                            + request.SortBy
                            + "' "
                            + request.SortOrder
                            + " OFFSET @Offset ROWS FETCH NEXT @PerPage ROWS ONLY;",
                        request
                    );
                    break;
            }

            if (schedules != null)
            {
                foreach (var schedule in schedules)
                {
                    GetUserHandler getUserHandler = new GetUserHandler(_dbService);
                    GetUserQuery getUserQuery = new GetUserQuery(
                        schedule.UserId,
                        new Guid(),
                        "Admin"
                    );
                    ActionResult<User?> user = await getUserHandler.Handle(
                        getUserQuery,
                        cancellationToken
                    );
                    schedule.User = user.Value!;
                }
            }
            return (schedules, count);
        }
    }
}
