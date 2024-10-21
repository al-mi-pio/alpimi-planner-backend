using alpimi_planner_backend.API;
using MediatR;

namespace AlpimiAPI.Entities.User.Queries
{
    public record GetUserByLoginQuery(string Login) : IRequest<User?>;

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
            var user = await _dbService.Get<User?>(
                "SELECT [Id], [Login], [CustomURL] FROM [User] WHERE [Login] = @Login;",
                request
            );
            return user;
        }
    }
}
