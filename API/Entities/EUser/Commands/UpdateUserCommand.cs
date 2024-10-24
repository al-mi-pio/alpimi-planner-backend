﻿using MediatR;

namespace AlpimiAPI.Entities.EUser.Commands
{
    public record UpdateUserCommand(
        Guid Id,
        string? Login,
        string? CustomURL,
        Guid FilteredID,
        string Role
    ) : IRequest<User?>;

    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, User?>
    {
        private readonly IDbService _dbService;

        public UpdateUserHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<User?> Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.CustomURL != null)
            {
                var userURL = await _dbService.Get<string>(
                    @"SELECT [CustomURL]
                FROM [User]
                WHERE [CustomURL] = @CustomURL;",
                    request
                );
                if (userURL != null)
                {
                    throw new BadHttpRequestException("URL already taken");
                }
            }
            User? user;
            switch (request.Role)
            {
                case "Admin":
                    user = await _dbService.Update<User?>(
                        @"
                    UPDATE [User] 
                    SET [Login]=CASE WHEN @Login IS NOT NULL THEN @Login 
                    ELSE [Login] END,[CustomURL]=CASE WHEN @CustomURL IS NOT NULL THEN @CustomURL ELSE [CustomURL] END 
                    OUTPUT INSERTED.[Id], INSERTED.[Login], INSERTED.[CustomURL]
                    WHERE [Id]=@Id;",
                        request
                    );
                    break;
                default:
                    user = await _dbService.Update<User?>(
                        @"
                    UPDATE [User] 
                    SET [Login]=CASE WHEN @Login IS NOT NULL THEN @Login 
                    ELSE [Login] END,[CustomURL]=CASE WHEN @CustomURL IS NOT NULL THEN @CustomURL ELSE [CustomURL] END 
                    OUTPUT INSERTED.[Id], INSERTED.[Login], INSERTED.[CustomURL]
                    WHERE [Id]=@Id AND [Id] = @FilteredID;",
                        request
                    );
                    break;
            }
            return user;
        }
    }
}
