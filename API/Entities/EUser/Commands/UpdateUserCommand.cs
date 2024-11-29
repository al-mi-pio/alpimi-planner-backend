using AlpimiAPI.Database;
using AlpimiAPI.Entities.EUser.DTO;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using MediatR;
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
            if (request.dto.CustomURL != null)
            {
                var userURL = await _dbService.Get<string>(
                    @"
                        SELECT 
                        [CustomURL]
                        FROM [User]
                        WHERE [CustomURL] = @CustomURL;",
                    request.dto
                );
                if (userURL != null)
                {
                    throw new ApiErrorException(
                        [new ErrorObject(_str["alreadyExists", "URL", request.dto.CustomURL])]
                    );
                }
            }
            User? user;
            switch (request.Role)
            {
                case "Admin":
                    user = await _dbService.Update<User?>(
                        $@"
                            UPDATE [User] 
                            SET
                            [Login]=CASE WHEN @CustomURL IS NOT NULL THEN @Login ELSE [Login] END,
                            [CustomURL]=CASE WHEN @CustomURL IS NOT NULL THEN @CustomURL ELSE [CustomURL] END 
                            OUTPUT 
                            INSERTED.[Id], 
                            INSERTED.[Login], 
                            INSERTED.[CustomURL]
                            WHERE [Id] = '{request.Id}';",
                        request.dto
                    );
                    break;
                default:
                    user = await _dbService.Update<User?>(
                        $@"
                            UPDATE [User] 
                            SET 
                            [Login]=CASE WHEN @Login IS NOT NULL THEN @Login ELSE [Login] END,
                            [CustomURL]=CASE WHEN @CustomURL IS NOT NULL THEN @CustomURL ELSE [CustomURL] END 
                            OUTPUT 
                            INSERTED.[Id], 
                            INSERTED.[Login], 
                            INSERTED.[CustomURL]
                            WHERE [Id] = '{request.Id}' AND [Id] = '{request.FilteredId}';",
                        request.dto
                    );
                    break;
            }
            return user;
        }
    }
}
