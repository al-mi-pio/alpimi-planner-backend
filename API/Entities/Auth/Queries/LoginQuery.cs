using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlpimiAPI.User;
using alpimi_planner_backend.API;
using alpimi_planner_backend.API.Utilities;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AlpimiAPI.Auth.Queries
{
    public record LoginQuery(string UserName, string Password) : IRequest<String>;

    public class LoginHandler
    {
        private readonly IPasswordHasher<Auth> _passwordHasher;

        private readonly IDbService _dbService;

        public LoginHandler(IDbService dbService, IPasswordHasher<Auth> passwordHasher)
        {
            _passwordHasher = passwordHasher;
            _dbService = dbService;
        }

        public async Task<String> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var auth = await _dbService.Get<Auth?>(
                @"SELECT [Auth].[Id],[Auth].[Password],[Auth].[UserID],[User].[Id],[User].[Login],[User].[CustomURL] 
                FROM [User] JOIN [Auth] on [User].[Id]=[Auth].[UserID] 
                WHERE [Login] = @UserName;",
                request
            );

            if (auth == null)
            {
                // TODO: add errors.xml file like in java tylko .json
                throw new BadHttpRequestException("Invalid login or password");
            }

            var result = _passwordHasher.VerifyHashedPassword(
                auth,
                auth.Password,
                request.Password
            );
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadHttpRequestException("Invaild password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, auth.Id.ToString()),
                new Claim("Login", $"{auth.User.Login}"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetJWTKey()));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //TODO rework
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
