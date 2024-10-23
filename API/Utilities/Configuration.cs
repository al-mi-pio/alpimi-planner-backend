using System.Security.Cryptography;

namespace AlpimiAPI.Utilities
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
        private static readonly string? _hashIterations = Environment.GetEnvironmentVariable(
            "HASH_ITERATIONS"
        );
        private static readonly string? _hashAlgorithm = Environment.GetEnvironmentVariable(
            "HASH_ALGORITHM"
        );
        private static readonly string? _keySize = Environment.GetEnvironmentVariable("KEY_SIZE");

        public static string? GetConnectionString()
        {
            if (_connectionString == null)
            {
                return "CONNECTION_STRING=\"Data Source=LMAO;Initial Catalog=testAdmin;Integrated Security=True;Pooling=False;Encrypt=True;Trust Server Certificate=True\"";
            }
            return _connectionString;
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

        public static int GetHashIterations()
        {
            if (_hashIterations == null)
            {
                return 10;
            }
            return Convert.ToInt32(_hashIterations);
        }

        public static int GetKeySize()
        {
            if (_keySize == null)
            {
                return 20;
            }
            return Convert.ToInt32(_keySize);
        }

        public static HashAlgorithmName GetHashAlgorithm()
        {
            if (_hashAlgorithm == null)
            {
                return HashAlgorithmName.SHA1;
            }
            return new HashAlgorithmName(_hashAlgorithm);
        }
    }
}
