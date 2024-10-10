using System.Data;
using alpimi_planner_backend.API.Utilities;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;

namespace AlpimiAPI.User.Commands
{
    public record PatchUserCommand(Guid Id, string? Login, string? CustomURL) : IRequest<User>;

    public class PatchUserHandler : IRequestHandler<PatchUserCommand, User>
    {
        public async Task<User> Handle(
            PatchUserCommand request,
            CancellationToken cancellationToken
        )
        {
            using (
                IDbConnection connection = new SqlConnection(Configuration.GetConnectionString())
            )
            {
                var user = await connection.QuerySingleOrDefaultAsync<User>(
                    @"
                    UPDATE [User] 
                    SET [Login]=CASE WHEN @Login IS NOT NULL THEN @Login 
                    ELSE [Login] END,[CustomURL]=CASE WHEN @CustomURL IS NOT NULL THEN @CustomURL ELSE [CustomURL] END 
                    OUTPUT INSERTED.[Id], INSERTED.[Login], INSERTED.[CustomURL]
                    WHERE [Id]=@Id;",
                    request
                );

                return user;
            }
        }
    }
}
