using System.Data;
using alpimi_planner_backend.API;
using alpimi_planner_backend.API.Utilities;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;

namespace AlpimiAPI.User.Queries
{
    public record GetUserQuery(Guid Id) : IRequest<User>;

    public class GetUserHandler : IRequestHandler<GetUserQuery, User>
    {
        private readonly IDbService _dbService;

        public GetUserHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _dbService.Get<User>(
                "SELECT [Id], [Login], [CustomURL] FROM [User] WHERE [Id] = @Id;",
                request
            );

            return user;
        }
    }
}
