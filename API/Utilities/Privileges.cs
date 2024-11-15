using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AlpimiAPI.Utilities
{
    public static class Privileges
    {
        public static Guid GetUserIdFromToken(string authorization)
        {
            var token = authorization.ToString().Split(" ").Last();
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);
            Claim userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")!;

            return Guid.Parse(userIdClaim.Value);
        }

        public static string GetUserRoleFromToken(string authorization)
        {
            var token = authorization.ToString().Split(" ").Last();
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);
            Claim RoleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)!;

            return (RoleClaim.Value);
        }

        public static string GetUserLoginFromToken(string authorization)
        {
            var token = authorization.ToString().Split(" ").Last();
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);
            Claim RoleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "login")!;

            return (RoleClaim.Value);
        }
    }
}
