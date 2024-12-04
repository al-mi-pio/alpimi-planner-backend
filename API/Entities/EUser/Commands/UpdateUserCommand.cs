using AlpimiAPI.Database;
using AlpimiAPI.Entities.EUser.DTO;
using AlpimiAPI.Entities.EUser.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Entities.EUser.Commands
{
    public record UpdateUserCommand(Guid Id, UpdateUserDTO dto, Guid FilteredId, string Role)
        : IRequest<User?>;

    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, User?>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;

        public UpdateUserHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<User?> Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken
        )
        {
            GetUserHandler getUserHandler = new GetUserHandler(_dbService);
            GetUserQuery getUserQuery = new GetUserQuery(
                request.Id,
                request.FilteredId,
                request.Role
            );
            ActionResult<User?> originalUser = await getUserHandler.Handle(
                getUserQuery,
                cancellationToken
            );

            if (originalUser.Value == null)
            {
                return null;
            }

            if (request.dto.CustomURL != null)
            {
                var userURL = await _dbService.Get<string>(
                    $@"
                        SELECT 
                        [CustomURL]
                        FROM [User]
                        WHERE [CustomURL] = @CustomURL AND [Id] != '{request.Id}';",
                    request.dto
                );
                if (userURL != null)
                {
                    throw new ApiErrorException(
                        [new ErrorObject(_str["alreadyExists", "URL", request.dto.CustomURL])]
                    );
                }
            }

            request.dto.CustomURL = request.dto.CustomURL ?? originalUser.Value.CustomURL;
            request.dto.Login = request.dto.Login ?? originalUser.Value.Login;

            User? user = await _dbService.Update<User?>(
                $@"
                UPDATE [User] 
                SET
                    [Login]=@Login, [CustomURL]=@CustomURL
                    OUTPUT 
                    INSERTED.[Id], 
                    INSERTED.[Login], 
                    INSERTED.[CustomURL]
                    WHERE [Id] = '{request.Id}';",
                request.dto
            );

            return user!;
        }
    }
}
