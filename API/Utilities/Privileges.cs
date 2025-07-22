using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AlpimiAPI.Utilities
{
    public static class Privileges
    {
        public static Guid GetUserIdFromToken(string? authorization)
        {
            if (authorization == null)
            {
                return new Guid();
            }

            var token = authorization.ToString().Split(" ").Last();
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);
            Claim userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")!;

            return Guid.Parse(userIdClaim.Value);
        }

        public static string GetUserRoleFromToken(string? authorization)
        {
            if (authorization == null)
            {
                return "Student";
            }

            var token = authorization.ToString().Split(" ").Last();
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);
            Claim userRoleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)!;

            return (userRoleClaim.Value);
        }

        public static string GetUserLoginFromToken(string authorization)
        {
            var token = authorization.ToString().Split(" ").Last();
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);
            Claim userLoginClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "login")!;

            return (userLoginClaim.Value);
        }
    }
}
