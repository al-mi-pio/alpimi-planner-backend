using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlpimiAPI.Breed;
using AlpimiAPI.Breed.Queries;
using alpimi_planner_backend.API.Utilities;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace alpimi_planner_backend.API.Entities.Auth.Queries
{
    public record LoginQuery(string UserName, string Password) : IRequest<Auth>;

    public class LoginHandler
    {
        private readonly IPasswordHasher<Auth> _passwordHasher;

        public LoginHandler(IPasswordHasher<Auth> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public async Task<String> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            User user;
            using (
                IDbConnection connection = new SqlConnection(Configuration.GetConnectionString())
            )
            {
                user = await connection.QueryFirstOrDefaultAsync<User>(
                    "SELECT [Id],[Login],[Password], FROM [User] WHERE [Login] = @UserName;",
                    request
                );
            }

            if (user == null)
            {
                // TODO: add errors.xml file like in java tylko .json
                throw new BadHttpRequestException("Invalid login or password");
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.Password,
                request.Password
            );
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadHttpRequestException("Invaild password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("Login", $"{user.Login}"),
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
