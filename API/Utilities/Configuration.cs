using alpimi_planner_backend.API.Settings;
using Newtonsoft.Json;

namespace alpimi_planner_backend.API.Utilities
{
    public class Configuration
    {
        private static readonly string? _connectionString = Environment.GetEnvironmentVariable(
            "CONNECTION_STRING"
        );
        private static readonly string? _jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
        private static readonly string? _jwtIssuer = Environment.GetEnvironmentVariable(
            "JWT_ISSUER"
        );
        private static readonly string? _jwtExpire = Environment.GetEnvironmentVariable(
            "JWT_EXPIRE"
        );
        private static readonly AuthConfiguration _authRequirements =
            JsonConvert.DeserializeObject<AuthConfiguration>(await JsonFileReader.ReadAsync<Item>(@"C:\myFile.json"););

        public static string? GetConnectionString()
        {
            return _connectionString;
        }

        public static AuthConfiguration GetAuthReqiurements()
        {
            return _authRequirements;
        }

        public static string GetJWTKey()
        {
            if (_jwtKey == null)
            {
                return "KeyNotFoundButThisMustBeLongEnough";
            }
            return _jwtKey;
        }

        public static string? GetJWTIssuer()
        {
            return _jwtIssuer;
        }

        public static double? GetJWTExpire()
        {
            return Convert.ToDouble(_jwtExpire);
        }
    }
}
