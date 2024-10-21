using System.Security.Cryptography;
using AlpimiAPI.User.Queries;
using alpimi_planner_backend.API;
using alpimi_planner_backend.API.Configuration;
using alpimi_planner_backend.API.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlpimiAPI.Entities.User.Commands
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
            GetUserByLoginHandler getUserByLoginHandler = new GetUserByLoginHandler(_dbService);
            GetUserByLoginQuery getUserByLoginQuery = new GetUserByLoginQuery(request.Login);
            ActionResult<User?> user = await getUserByLoginHandler.Handle(
                getUserByLoginQuery,
                cancellationToken
            );

            if (user.Value != null)
            {
                throw new BadHttpRequestException("Login already taken");
            }

            if (request.Password.Length < AuthConfiguration.MinimumPasswordLength)
            {
                throw new BadHttpRequestException(
                    "Password cannot be shorter than "
                        + AuthConfiguration.MinimumPasswordLength
                        + " characters"
                );
            }

            if (request.Password.Length > AuthConfiguration.MaximumPasswordLength)
            {
                throw new BadHttpRequestException(
                    "Password cannot be longer than "
                        + AuthConfiguration.MaximumPasswordLength
                        + " characters"
                );
            }

            RequiredCharacterTypes[]? requiredCharacterTypes = AuthConfiguration.RequiredCharacters;
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
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                request.Password,
                salt,
                Configuration.GetHashIterations(),
                Configuration.GetHashAlgorithm(),
                Configuration.GetKeySize()
            );

            await _dbService.Post<Guid>(
                @"
                    INSERT INTO [Auth] ([Id],[Password],[Salt],[UserID])
                    OUTPUT INSERTED.UserID                    
                    VALUES (@AuthId,'"
                    + Convert.ToBase64String(hash)
                    + "','"
                    + Convert.ToBase64String(salt)
                    + "',@Id);",
                request
            );

            return insertedId;
        }
    }
}
