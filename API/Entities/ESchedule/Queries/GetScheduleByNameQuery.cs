using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ESchedule.Queries
{
    public record GetScheduleByNameQuery(string Name, Guid FilteredID, string Role)
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
                        "SELECT [Id], [Name], [SchoolHour], [UserID] FROM [Schedule] WHERE [Name] = @Name;",
                        request
                    );
                    break;
                default:
                    schedule = await _dbService.Get<Schedule?>(
                        "SELECT [Id], [Name], [SchoolHour], [UserID] FROM [Schedule] WHERE [Name] = @Name AND [UserId] = @FilteredID;",
                        request
                    );
                    break;
            }
            GetUserHandler getUserHandler = new GetUserHandler(_dbService);
            GetUserQuery getUserQuery = new GetUserQuery(request.FilteredID, new Guid(), "Admin");
            ActionResult<User?> user = await getUserHandler.Handle(getUserQuery, cancellationToken);
            if (schedule != null)
            {
                schedule.User = user.Value!;
            }
            return schedule;
        }
    }
}
