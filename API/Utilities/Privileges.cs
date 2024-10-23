using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AlpimiAPI.Utilities
{
    public static class Privileges
    {
        public static Guid GetUserIDFromToken(string authorization)
        {
            var token = authorization.ToString().Split(" ").Last();
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);
            Claim userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userID")!;

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
    }
}
