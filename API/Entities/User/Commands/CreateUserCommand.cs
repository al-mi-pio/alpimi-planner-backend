using System;
using System.Security.Cryptography;
using System.Text;
using alpimi_planner_backend.API;
using alpimi_planner_backend.API.Configuration;
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
            var passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(request.Password));

            AuthConfiguration authConfig = new AuthConfiguration();

            if (request.Password.Length < authConfig.GetMinimumPasswordLength())
            {
                throw new BadHttpRequestException(
                    "Password cannot be shorter than "
                        + authConfig.GetMinimumPasswordLength()
                        + " characters"
                );
            }

            if (request.Password.Length > authConfig.GetMaximumPasswordLength())
            {
                throw new BadHttpRequestException(
                    "Password cannot be longer than "
                        + authConfig.GetMaximumPasswordLength()
                        + " characters"
                );
            }

            RequiredCharacterTypes[]? requiredCharacterTypes = authConfig.GetRequiredCharacters();
            if (requiredCharacterTypes != null)
            {
                if (requiredCharacterTypes.Contains(RequiredCharacterTypes.BigLetter))
                {
                    if (!request.Password.Any(char.IsUpper))
                    {
                        throw new BadHttpRequestException(
                            "Password must contain at least one of the following: "
                                + string.Join(", ", requiredCharacterTypes)
                        );
                    }
                }
                if (requiredCharacterTypes.Contains(RequiredCharacterTypes.SmallLetter))
                {
                    if (!request.Password.Any(char.IsLower))
                    {
                        throw new BadHttpRequestException(
                            "Password must contain at least one of the following: "
                                + string.Join(", ", requiredCharacterTypes)
                        );
                    }
                }
                if (requiredCharacterTypes.Contains(RequiredCharacterTypes.Digit))
                {
                    if (!request.Password.Any(char.IsDigit))
                    {
                        throw new BadHttpRequestException(
                            "Password must contain at least one of the following: "
                                + string.Join(", ", requiredCharacterTypes)
                        );
                    }
                }
                if (requiredCharacterTypes.Contains(RequiredCharacterTypes.Symbol))
                {
                    if (
                        !(
                            request.Password.Any(char.IsSymbol)
                            || request.Password.Any(char.IsPunctuation)
                        )
                    )
                    {
                        throw new BadHttpRequestException(
                            "Password must contain at least one of the following: "
                                + string.Join(", ", requiredCharacterTypes)
                        );
                    }
                }
            }

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
                    VALUES (@AuthId,'"
                    + Convert.ToHexString(passwordHash)
                    + "',@Id);",
                request
            );

            return insertedId;
        }
    }
}
