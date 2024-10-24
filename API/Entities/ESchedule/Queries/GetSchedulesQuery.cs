using System.Collections.Generic;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.ESchedule.Queries
{
    public record GetSchedulesQuery(Guid FilteredID, string Role)
        : IRequest<IEnumerable<Schedule>?>;

    public class GetSchedulesHandler : IRequestHandler<GetSchedulesQuery, IEnumerable<Schedule>?>
    {
        private readonly IDbService _dbService;

        public GetSchedulesHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<IEnumerable<Schedule>?> Handle(
            GetSchedulesQuery request,
            CancellationToken cancellationToken
        )
        {
            IEnumerable<Schedule>? schedules;
            switch (request.Role)
            {
                case "Admin":
                    schedules = await _dbService.GetAll<Schedule>(
                        "SELECT [Id], [Name], [SchoolHour], [UserID] FROM [Schedule];",
                        request
                    );
                    break;
                default:
                    schedules = await _dbService.GetAll<Schedule>(
                        "SELECT [Id], [Name], [SchoolHour], [UserID] FROM [Schedule] where [UserId] = @FilteredID;",
                        request
                    );
                    break;
            }
            GetUserHandler getUserHandler = new GetUserHandler(_dbService);
            GetUserQuery getUserQuery = new GetUserQuery(request.FilteredID, new Guid(), "Admin");
            ActionResult<User?> user = await getUserHandler.Handle(getUserQuery, cancellationToken);
            if (schedules != null)
            {
                foreach (var schedule in schedules)
                {
                    schedule.User = user.Value!;
                }
            }
            return schedules;
        }
    }
}
