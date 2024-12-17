using AlpimiAPI.Database;
using MediatR;

namespace AlpimiAPI.Entities.EUser.Queries
{
    public record GetUserQuery(Guid Id, Guid FilteredId, string Role) : IRequest<User?>;

    public class GetUserHandler : IRequestHandler<GetUserQuery, User?>
    {
        private readonly IDbService _dbService;

        public GetUserHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<User?> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            User? user;
            switch (request.Role)
            {
                case "Admin":
                    user = await _dbService.Get<User?>(
                        @"
                            SELECT
                            [Id], [Login], [CustomURL] 
                            FROM [User] 
                            WHERE [Id] = @Id;",
                        request
                    );
                    break;
                default:
                    user = await _dbService.Get<User?>(
                        @"
                            SELECT 
                            [Id], [Login], [CustomURL] 
                            FROM [User] 
                            WHERE [Id] = @Id AND [Id] = @FilteredId;",
                        request
                    );
                    break;
            }

            return user;
        }
    }
}
