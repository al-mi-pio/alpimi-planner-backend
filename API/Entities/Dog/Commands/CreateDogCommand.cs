using MediatR;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using Azure.Core;
using alpimi_planner_backend.API.Utilities;

namespace AlpimiAPI.Dog.Commands
{
    public record CreateDogCommand(string Name, DateTime BirthDate, int BreedId) : IRequest<int>;

    public class CreateDogHandler : IRequestHandler<CreateDogCommand, int>
    {
        public async Task<int> Handle(CreateDogCommand request, CancellationToken cancellationToken)
        {
            using (IDbConnection connection = new SqlConnection(Configuration.GetConnectionString()))
            {
                await connection.QueryFirstOrDefaultAsync<AlpimiAPI.Breed.Breed>("SELECT [Id], [Name], [CountryOrigin] FROM [Breed] WHERE [Id] = @Id;", new { Id = request.BreedId});
                
                var insertedId = await connection.QuerySingleOrDefaultAsync<int>(@"
                    INSERT INTO [Dog] ([Name], [BirthDate], [BreedId])
                    OUTPUT INSERTED.Id                    
                    VALUES (@Name, @BirthDate, @BreedId);", request);

                return insertedId;
            }
        }
    }
}
