using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlpimiAPI.Utilities;
using Microsoft.IdentityModel.Tokens;

namespace AlpimiTest.TestUtilities
{
    public static class TestAuthorization
    {
        public static string GetToken(string Role, string Login, Guid UserId)
        {
            var claims = new List<Claim>()
            {
                new Claim("login", Login),
                new Claim("userId", Convert.ToString(UserId)!),
            };

            switch (Role)
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
