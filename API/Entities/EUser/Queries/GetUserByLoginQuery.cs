using MediatR;

namespace AlpimiAPI.Entities.EUser.Queries
{
    public record GetUserByLoginQuery(string Login, Guid? FilterID) : IRequest<User?>;

    public class GetUserByLoginHandler : IRequestHandler<GetUserByLoginQuery, User?>
    {
        private readonly IDbService _dbService;

        public GetUserByLoginHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<User?> Handle(
            GetUserByLoginQuery request,
            CancellationToken cancellationToken
        )
        {
            User? user;
            if (request.FilterID == null)
            {
                user = await _dbService.Get<User?>(
                    "SELECT [Id], [Login], [CustomURL] FROM [User] WHERE [Login] = @Login;",
                    request
                );
            }
            else
            {
                user = await _dbService.Get<User?>(
                    "SELECT [Id], [Login], [CustomURL] FROM [User] WHERE [Login] = @Login AND [Id] = @FilterID;",
                    request
                );
            }
            return user;
        }
    }
}
