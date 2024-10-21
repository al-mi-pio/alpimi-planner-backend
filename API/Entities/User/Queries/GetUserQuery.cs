using alpimi_planner_backend.API;
using MediatR;

namespace AlpimiAPI.Entities.User.Queries
{
    public record GetUserQuery(Guid Id) : IRequest<User?>;

    public class GetUserHandler : IRequestHandler<GetUserQuery, User?>
    {
        private readonly IDbService _dbService;

        public GetUserHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<User?> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _dbService.Get<User?>(
                "SELECT [Id], [Login], [CustomURL] FROM [User] WHERE [Id] = @Id;",
                request
            );
            return user;
        }
    }
}
