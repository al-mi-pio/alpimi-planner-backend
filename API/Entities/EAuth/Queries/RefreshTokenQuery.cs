using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlpimiAPI.Utilities;
using Microsoft.IdentityModel.Tokens;

namespace AlpimiAPI.Entities.EAuth.Queries
{
    public record RefreshTokenQuery(string Login, Guid UserId, string Role);

    public class RefreshTokenHandler
    {
        public string Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
        {
            var claims = new List<Claim>()
            {
                new Claim("login", $"{request.Login}"),
                new Claim("userId", $"{request.UserId}")
            };
            switch (request.Role)
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
