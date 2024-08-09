using MediatR;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using alpimi_planner_backend.API.Utilities;

namespace AlpimiAPI.Breed.Commands
{
    public record CreateBreedCommand(string Name, string? CountryOrigin) : IRequest<int>;

    public class CreateBreedHandler : IRequestHandler<CreateBreedCommand, int>
    {
        public async Task<int> Handle(CreateBreedCommand request, CancellationToken cancellationToken)
        {
            using (IDbConnection connection = new SqlConnection(Configuration.GetConnectionString()))
            {
                var insertedId = await connection.QuerySingleOrDefaultAsync<int>(@"
                    INSERT INTO [Breed] ([Name], [CountryOrigin])
                    OUTPUT INSERTED.Id                    
                    VALUES (@Name, @CountryOrigin);", request);

                return insertedId;
            }

        }
    }
}
