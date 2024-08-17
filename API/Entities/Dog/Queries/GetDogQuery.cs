using System.Data;
using System.Data.Common;
using alpimi_planner_backend.API.Utilities;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;

namespace AlpimiAPI.Dog.Queries
{
    public record GetDogQuery(int Id) : IRequest<Dog>;

    public class GetDogHandler : IRequestHandler<GetDogQuery, Dog>
    {
        public async Task<Dog> Handle(GetDogQuery request, CancellationToken cancellationToken)
        {
            using (
                IDbConnection connection = new SqlConnection(Configuration.GetConnectionString())
            )
            {
                var dog = await connection.QueryAsync<Dog, Breed.Breed, Dog>(
                    @"
                    SELECT  [Dog].[Id], [Dog].[Name], [Dog].[BirthDate], [Dog].[BreedId], 
                            [Breed].[Id], [Breed].[Name], [Breed].[CountryOrigin]
                    FROM [Dog] 
                    LEFT JOIN [Breed] on [Breed].[Id] = [Dog].[BreedId]
                    WHERE [Dog].[Id] = @Id;",
                    (dog, breed) =>
                    {
                        dog.Breed = breed;
                        return dog;
                    },
                    request,
                    splitOn: "Id"
                );

                return dog.FirstOrDefault();
            }
        }
    }
}
