using MediatR;

namespace AlpimiAPI.Entities.EUser.Queries
{
    public record GetUserQuery(Guid Id, Guid? FilterID) : IRequest<User?>;

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
            if (request.FilterID == null)
            {
                user = await _dbService.Get<User?>(
                    "SELECT [Id], [Login], [CustomURL] FROM [User] WHERE [Id] = @Id;",
                    request
                );
            }
            else
            {
                user = await _dbService.Get<User?>(
                    "SELECT [Id], [Login], [CustomURL] FROM [User] WHERE [Id] = @Id AND [Id] = @FilterID;",
                    request
                );
            }
            return user;
        }
    }
}
