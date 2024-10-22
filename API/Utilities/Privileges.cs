using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AlpimiAPI.Utilities
{
    public static class Privileges
    {
        public static bool CheckOwnership(string authorization, Guid id)
        {
            if (authorization is null)
            {
                return false;
            }

            var token = authorization.ToString().Split(" ").Last();
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);
            Claim userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userID")!;

            if (Guid.Parse(userIdClaim.Value) != id)
            {
                return false;
            }
            return true;
        }
    }
}
