using System.Data;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;

namespace AlpimiAPI.Breed.Queries
{
    public record GetBreedQuery(int Id) : IRequest<Breed>;

    public class GetBreedHandler : IRequestHandler<GetBreedQuery, Breed>
    {
        public async Task<Breed> Handle(GetBreedQuery request, CancellationToken cancellationToken)
        {
            using (
                IDbConnection connection = new SqlConnection(
                    Utilities.Configuration.GetConnectionString()
                )
            )
            {
                var breed = await connection.QueryFirstOrDefaultAsync<Breed>(
                    "SELECT [Id], [Name], [CountryOrigin] FROM [Breed] WHERE [Id] = @Id;",
                    request
                );

                return breed;
            }
        }
    }
}
