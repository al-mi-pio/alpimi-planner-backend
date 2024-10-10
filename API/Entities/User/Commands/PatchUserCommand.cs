using System.Data;
using alpimi_planner_backend.API.Utilities;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;

namespace AlpimiAPI.User.Commands
{
    public record PatchUserCommand(Guid Id, string? Login, string? CustomURL) : IRequest<Guid>;

    public class PatchUserHandler : IRequestHandler<PatchUserCommand, Guid>
    {
        public async Task<Guid> Handle(
            PatchUserCommand request,
            CancellationToken cancellationToken
        )
        {
            using (
                IDbConnection connection = new SqlConnection(Configuration.GetConnectionString())
            )
            {
                var changedId = await connection.QuerySingleOrDefaultAsync<Guid>(
                    @"
                    UPDATE [User] 
                    SET [Login]=CASE WHEN @Login IS NOT NULL THEN @Login 
                    ELSE [Login] END,[CustomURL]=CASE WHEN @CustomURL IS NOT NULL THEN @CustomURL ELSE [CustomURL] END 
                    WHERE [Id]=@Id;",
                    request
                );

                return changedId;
            }
        }
    }
}
