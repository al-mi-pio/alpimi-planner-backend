using alpimi_planner_backend.API.Utilities;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AlpimiAPI.Breed.Commands
{
    public record DeleteBreedCommand(int Id): IRequest;

    public class DeleteBreedHandler : IRequestHandler<DeleteBreedCommand>
    {
        public async Task Handle(DeleteBreedCommand request, CancellationToken cancellationToken)
        {
            using (IDbConnection connection = new SqlConnection(Configuration.GetConnectionString()))
            {
                await connection.ExecuteAsync("DELETE [Breed] WHERE [Id] = @Id;", request);
            }
        }
    }
}
