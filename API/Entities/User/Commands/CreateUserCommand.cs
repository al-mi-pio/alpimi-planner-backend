using alpimi_planner_backend.API;
using MediatR;

namespace AlpimiAPI.User.Commands
{
    public record CreateUserCommand(
        Guid Id,
        Guid AuthId,
        string Login,
        string CustomURL,
        string Password
    ) : IRequest<Guid>;

    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IDbService _dbService;

        public CreateUserHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<Guid> Handle(
            CreateUserCommand request,
            CancellationToken cancellationToken
        )
        {
            var insertedId = await _dbService.Post<Guid>(
                @"
                    INSERT INTO [User] ([Id],[Login],[CustomURL])
                    OUTPUT INSERTED.Id                    
                    VALUES (@Id,@Login,@CustomURL);",
                request
            );
            await _dbService.Post<Guid>(
                @"
                    INSERT INTO [Auth] ([Id],[Password],[UserID])
                    OUTPUT INSERTED.UserID                    
                    VALUES (@AuthId,@Password,@Id);",
                request
            );

            return insertedId;
        }
    }
}
