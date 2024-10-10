using System.Data;
using alpimi_planner_backend.API.Utilities;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;

namespace AlpimiAPI.User.Queries
{
    public record GetUserQuery(Guid Id) : IRequest<User>;

    public class GetBreedHandler : IRequestHandler<GetUserQuery, User>
    {
        public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            using (
                IDbConnection connection = new SqlConnection(Configuration.GetConnectionString())
            )
            {
                var user = await connection.QueryFirstOrDefaultAsync<User>(
                    "SELECT [Id], [Login], [CustomURL] FROM [User] WHERE [Id] = @Id;",
                    request
                );

                return user;
            }
        }
    }
}
