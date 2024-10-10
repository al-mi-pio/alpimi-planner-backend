using System.Data;
using alpimi_planner_backend.API.Utilities;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;

namespace AlpimiAPI.User.Commands
{
    public record CreateUserCommand(Guid Id, string Login, string Password, string CustomURL)
        : IRequest<Guid>;

    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        public async Task<Guid> Handle(
            CreateUserCommand request,
            CancellationToken cancellationToken
        )
        {
            using (
                IDbConnection connection = new SqlConnection(Configuration.GetConnectionString())
            )
            {
                var insertedId = await connection.QuerySingleOrDefaultAsync<Guid>(
                    @"
                    INSERT INTO [User] ([Id],[Login], [Password],[CustomURL])
                    OUTPUT INSERTED.Id                    
                    VALUES (@Id,@Login,@Password,@CustomURL);",
                    request
                );

                return insertedId;
            }
        }
    }
}
