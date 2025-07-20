using System.Security.Cryptography;
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
    public record CreateUserCommand(Guid Id, Guid AuthId, CreateUserDTO dto) : IRequest<Guid>;

    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IDbService _dbService;
        private readonly IStringLocalizer<Errors> _str;
        private readonly IStringLocalizer<Fields> _strFields;

        public CreateUserHandler(
            IDbService dbService,
            IStringLocalizer<Errors> str,
            IStringLocalizer<Fields> strFields
        )
        {
            _dbService = dbService;
            _str = str;
            _strFields = strFields;
        }

        public async Task<Guid> Handle(
            CreateUserCommand request,
            CancellationToken cancellationToken
        )
        {
            GetUserByLoginHandler getUserByLoginHandler = new GetUserByLoginHandler(_dbService);
            GetUserByLoginQuery getUserByLoginQuery = new GetUserByLoginQuery(
                request.dto.Login!,
                new Guid(),
                "Admin"
            );
            ActionResult<User?> user = await getUserByLoginHandler.Handle(
                getUserByLoginQuery,
                cancellationToken
            );

            List<ErrorObject> errors = new List<ErrorObject>();
            if (user.Value != null)
            {
                errors.Add(
                    new FieldErrorObject(
                        "name",
                        _str["alreadyExists", _strFields["User"], request.dto.Login!]
                    )
                );
            }

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
                errors.Add(
                    new FieldErrorObject(
                        "name",
                        _str["alreadyExists", _strFields["CustomUrl"], request.dto.CustomURL!]
                    )
                );
            }

            if (request.dto.Password!.Length < AuthSettings.MinimumPasswordLength)
            {
                errors.Add(
                    new ErrorObject(_str["shortPassword", AuthSettings.MinimumPasswordLength])
                );
            }

            if (request.dto.Password.Length > AuthSettings.MaximumPasswordLength)
            {
                errors.Add(
                    new ErrorObject(_str["longPassword", AuthSettings.MaximumPasswordLength])
                );
            }

            RequiredCharacterTypes[]? requiredCharacterTypes =
                Configuration.GetRequiredCharacterTypesForPassword();

            if (!CharacterFilter.Required(request.dto.Password, requiredCharacterTypes))
            {
                errors.Add(
                    new ErrorObject(
                        _str["passwordMustContain", string.Join(", ", requiredCharacterTypes!)]
                    )
                );
            }

            AllowedCharacterTypes[]? allowedCharacterTypesForPassword =
                Configuration.GetAllowedCharacterTypesForPassword();

            if (!CharacterFilter.Allowed(request.dto.Password, allowedCharacterTypesForPassword))
            {
                errors.Add(
                    new ErrorObject(
                        _str[
                            "cantContain",
                            "Password",
                            string.Join(", ", allowedCharacterTypesForPassword!)
                        ]
                    )
                );
            }

            AllowedCharacterTypes[]? allowedCharacterTypesForLogin =
                Configuration.GetAllowedCharacterTypesForLogin();

            if (!CharacterFilter.Allowed(request.dto.Login!, allowedCharacterTypesForLogin))
            {
                errors.Add(
                    new ErrorObject(
                        _str[
                            "cantContain",
                            "Login",
                            string.Join(", ", allowedCharacterTypesForLogin!)
                        ]
                    )
                );
            }

            AllowedCharacterTypes[]? allowedCharacterTypesForCustomURL =
                Configuration.GetAllowedCharacterTypesForCustomURL();

            if (!CharacterFilter.Allowed(request.dto.CustomURL!, allowedCharacterTypesForCustomURL))
            {
                errors.Add(
                    new ErrorObject(
                        _str[
                            "cantContain",
                            "CustomURL",
                            string.Join(", ", allowedCharacterTypesForCustomURL!)
                        ]
                    )
                );
            }

            if (errors.Count != 0)
            {
                throw new ApiErrorException(errors);
            }

            var insertedId = await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [User] 
                    ([Id], [Login], [CustomURL])
                    OUTPUT 
                    INSERTED.Id                    
                    VALUES (
                    '{request.Id}',
                    @Login,
                    @CustomURL);",
                request.dto
            );

            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                request.dto.Password,
                salt,
                Configuration.GetHashIterations(),
                Configuration.GetHashAlgorithm(),
                Configuration.GetKeySize()
            );

            await _dbService.Post<Guid>(
                $@"
                    INSERT INTO [Auth] 
                    ([Id], [Password], [Salt], [Role], [UserId])
                    OUTPUT 
                    INSERTED.UserId                    
                    VALUES (
                    @AuthId,
                    '{Convert.ToBase64String(hash)}',
                    '{Convert.ToBase64String(salt)}',
                    'User',
                    @Id);",
                request
            );

            return insertedId;
        }
    }
}
