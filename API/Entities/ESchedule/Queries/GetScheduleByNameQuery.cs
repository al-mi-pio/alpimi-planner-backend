using AlpimiAPI.Database;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ESchedule.Queries
{
    public record GetScheduleByNameQuery(string Name, Guid FilteredId, string Role)
        : IRequest<Schedule?>;

    public class GetScheduleByNameHandler : IRequestHandler<GetScheduleByNameQuery, Schedule?>
    {
        private readonly IDbService _dbService;

        public GetScheduleByNameHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<Schedule?> Handle(
            GetScheduleByNameQuery request,
            CancellationToken cancellationToken
        )
        {
            Schedule? schedule;
            switch (request.Role)
            {
                case "Admin":
                    schedule = await _dbService.Get<Schedule?>(
                        @"
                            SELECT
                            [Id], [Name], [UserId] 
                            FROM [Schedule] 
                            WHERE [Name] = @Name;",
                        request
                    );
                    break;
                default:
                    schedule = await _dbService.Get<Schedule?>(
                        @"
                            SELECT 
                            [Id], [Name], [UserId] 
                            FROM [Schedule] 
                            WHERE [Name] =  @Name AND [UserId] = @FilteredId;",
                        request
                    );
                    break;
            }

            if (schedule != null)
            {
                GetUserHandler getUserHandler = new GetUserHandler(_dbService);
                GetUserQuery getUserQuery = new GetUserQuery(schedule.UserId, new Guid(), "Admin");
                ActionResult<User?> user = await getUserHandler.Handle(
                    getUserQuery,
                    cancellationToken
                );
                schedule.User = user.Value!;
            }
            return schedule;
        }
    }
}
