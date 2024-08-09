using System.Data;
using alpimi_planner_backend.API.Utilities;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;

namespace AlpimiAPI.Breed.Queries
{
    public record GetBreedsQuery : IRequest<IEnumerable<Breed>>;

    public class GetBreedsHandler : IRequestHandler<GetBreedsQuery, IEnumerable<Breed>>
    {
        public async Task<IEnumerable<Breed>> Handle(
            GetBreedsQuery request,
            CancellationToken cancellationToken
        )
        {
            using (
                IDbConnection connection = new SqlConnection(Configuration.GetConnectionString())
            )
            {
                var breeds = await connection.QueryAsync<Breed>(
                    "SELECT [Id], [Name], [CountryOrigin] FROM [Breed];"
                );

                return breeds;
            }
        }
    }
}
