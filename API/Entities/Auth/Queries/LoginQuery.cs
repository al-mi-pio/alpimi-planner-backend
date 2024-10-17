using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using alpimi_planner_backend.API;
using alpimi_planner_backend.API.Utilities;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace AlpimiAPI.Auth.Queries
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
                @"SELECT [Auth].[Id],[Auth].[Password],[Auth].[UserID]
                FROM [User] JOIN [Auth] on [User].[Id]=[Auth].[UserID] 
                WHERE [Login] = @Login;",
                request
            );
            var user = await _dbService.Post<User.User?>(
                @"SELECT [Id],[Login],[CustomURL]
                FROM [User] 
                WHERE [Login] = @Login;",
                request
            );

            if (auth == null || user == null)
            {
                throw new BadHttpRequestException("Invalid login or password");
            }
            auth.User = user;
            if (
                auth.Password
                != Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.Password)))
            )
            {
                throw new BadHttpRequestException("Invalid password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, auth.UserID.ToString()),
                new Claim("login", $"{auth.User.Login}"),
                new Claim("userID", $"{auth.UserID}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetJWTKey()));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //TODO DO SMTH
            var expires = DateTime.Now;
            if (Configuration.GetJWTExpire() != null)
            {
                expires = DateTime.Now.AddMinutes(Convert.ToDouble(Configuration.GetJWTExpire()));
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
