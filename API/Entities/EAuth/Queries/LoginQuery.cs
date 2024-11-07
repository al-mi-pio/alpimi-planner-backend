using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Entities.EUser.Queries;
using AlpimiAPI.Responses;
using AlpimiAPI.Utilities;
using alpimi_planner_backend.API.Locales;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;

namespace AlpimiAPI.Entities.EAuth.Queries
{
    public record LoginQuery(string Login, string Password) : IRequest<String>;

    public class LoginHandler : IRequestHandler<LoginQuery, string>
    {
        private readonly IDbService _dbService;

        private readonly IStringLocalizer<Errors> _str;

        public LoginHandler(IDbService dbService, IStringLocalizer<Errors> str)
        {
            _dbService = dbService;
            _str = str;
        }

        public async Task<String> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var auth = await _dbService.Post<Auth?>(
                @"SELECT [Auth].[Id],[Auth].[Password],[Auth].[Salt],[Auth].[Role],[Auth].[UserId]
                FROM [User] JOIN [Auth] on [User].[Id]=[Auth].[UserId] 
                WHERE [Login] = @Login;",
                request
            );

            GetUserByLoginHandler getUserByLoginHandler = new GetUserByLoginHandler(_dbService);
            GetUserByLoginQuery getUserByLoginQuery = new GetUserByLoginQuery(
                request.Login,
                new Guid(),
                "Admin"
            );
            ActionResult<User?> user = await getUserByLoginHandler.Handle(
                getUserByLoginQuery,
                cancellationToken
            );

            if (auth == null || user.Value == null)
            {
                throw new ApiErrorException([new ErrorObject(_str["invalidLoginOrPassword"])]);
            }
            auth.User = user.Value;

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                request.Password,
                Convert.FromBase64String(auth.Salt),
                Configuration.GetHashIterations(),
                Configuration.GetHashAlgorithm(),
                Configuration.GetKeySize()
            );

            if (Convert.ToBase64String(inputHash) != auth.Password)
            {
                throw new ApiErrorException([new ErrorObject(_str["invalidPassword"])]);
            }

            var claims = new List<Claim>()
            {
                new Claim("login", $"{auth.User.Login}"),
                new Claim("userId", $"{auth.UserId}")
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetJWTKey()));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddMinutes(Configuration.GetJWTExpire());

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
