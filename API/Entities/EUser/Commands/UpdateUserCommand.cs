using AlpimiAPI.Database;
using AlpimiAPI.Entities.EUser.DTO;
using AlpimiAPI.Entities.EUser.Queries;
using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiAPI.Utilities;
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
        private readonly IStringLocalizer<Fields> _strFields;

        public UpdateUserHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
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
                AllowedCharacterTypes[]? allowedCharacterTypesForCustomURL =
                    Configuration.GetAllowedCharacterTypesForCustomURL();

                if (
                    !CharacterFilter.Allowed(
                        request.dto.CustomURL,
                        allowedCharacterTypesForCustomURL
                    )
                )
                {
                    throw new ApiErrorException(
                        [
                            new ErrorObject(
                                _str[
                                    "cantContain",
                                    "CustomURL",
                                    string.Join(", ", allowedCharacterTypesForCustomURL!)
                                ]
                            )
                        ]
                    );
                }

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
                        [
                            new FieldErrorObject(
                                "name",
                                _str[
                                    "alreadyExists",
                                    _strFields["CustomUrl"],
                                    request.dto.CustomURL
                                ]
                            )
                        ]
                    );
                }
            }

            if (request.dto.Login != null)
            {
                AllowedCharacterTypes[]? allowedCharacterTypesForLogin =
                    Configuration.GetAllowedCharacterTypesForLogin();

                if (!CharacterFilter.Allowed(request.dto.Login, allowedCharacterTypesForLogin))
                {
                    throw new ApiErrorException(
                        [
                            new ErrorObject(
                                _str[
                                    "cantContain",
                                    "Login",
                                    string.Join(", ", allowedCharacterTypesForLogin!)
                                ]
                            )
                        ]
                    );
                }

                var userLogin = await _dbService.GetAll<string>(
                    $@"
                        SELECT 
                        [Login]
                        FROM [User]
                        WHERE [Login] = @Login AND [Id] != '{request.Id}';",
                    request.dto
                );
                if (userLogin!.Any())
                {
                    throw new ApiErrorException(
                        [
                            new FieldErrorObject(
                                "name",
                                _str["alreadyExists", _strFields["Login"], request.dto.Login]
                            )
                        ]
                    );
                }
            }

            request.dto.CustomURL = request.dto.CustomURL ?? originalUser.Value.CustomURL;
            request.dto.Login = request.dto.Login ?? originalUser.Value.Login;

            var user = await _dbService.Update<User?>(
                $@"
                    UPDATE [User] 
                    SET
                    [Login] = @Login, [CustomURL] = @CustomURL
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
