using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Queries;
using AlpimiAPI.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AlpimiAPI.Entities.EAuth.Queries
{
    public record LoginQuery(string Login, string Password) : IRequest<String>;

    public class LoginHandler : IRequestHandler<LoginQuery, string>
    {
        private readonly IDbService _dbService;

        public LoginHandler(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<String> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var auth = await _dbService.Post<Auth?>(
                @"SELECT [Auth].[Id],[Auth].[Password],[Auth].[Salt],[Auth].[Role],[Auth].[UserID]
                FROM [User] JOIN [Auth] on [User].[Id]=[Auth].[UserID] 
                WHERE [Login] = @Login;",
                request
            );

            GetUserByLoginHandler getUserByLoginHandler = new GetUserByLoginHandler(_dbService);
            GetUserByLoginQuery getUserByLoginQuery = new GetUserByLoginQuery(request.Login);
            ActionResult<User?> user = await getUserByLoginHandler.Handle(
                getUserByLoginQuery,
                cancellationToken
            );

            if (auth == null || user.Value == null)
            {
                throw new BadHttpRequestException("Invalid login or password");
            }
            auth.User = user.Value;

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                request.Password,
                Convert.FromBase64String(auth.Salt),
                Utilities.Configuration.GetHashIterations(),
                Utilities.Configuration.GetHashAlgorithm(),
                Utilities.Configuration.GetKeySize()
            );

            if (Convert.ToBase64String(inputHash) != auth.Password)
            {
                throw new BadHttpRequestException("Invalid password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, auth.UserID.ToString()),
                new Claim("login", $"{auth.User.Login}"),
                new Claim("userID", $"{auth.UserID}")
            };
            switch (auth.Role)
            {
                case "Admin":
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    break;
                case "User":
                    claims.Add(new Claim(ClaimTypes.Role, "User"));
                    break;
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Utilities.Configuration.GetJWTKey())
            );
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //TODO DO SMTH
            var expires = DateTime.Now;
            if (Utilities.Configuration.GetJWTExpire() != null)
            {
                expires = DateTime.Now.AddMinutes(
                    Convert.ToDouble(Utilities.Configuration.GetJWTExpire())
                );
            }
            else
            {
                expires = DateTime.Now.AddMinutes(1000.0);
            }

            var token = new JwtSecurityToken(
                Configuration.GetJWTIssuer(),
                Configuration.GetJWTIssuer(),
                claims,
                expires: expires,
                signingCredentials: cred
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }
    }
}
