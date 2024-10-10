using System.Data;
using alpimi_planner_backend.API.Utilities;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;

namespace AlpimiAPI.User.Commands
{
    public record DeleteUserCommand(Guid Id) : IRequest;

    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
    {
        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            using (
                IDbConnection connection = new SqlConnection(Configuration.GetConnectionString())
            )
            {
                await connection.ExecuteAsync("DELETE [User] WHERE [Id] = @Id;", request);
            }
        }
    }
}
