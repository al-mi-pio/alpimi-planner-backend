using System.Security.Cryptography;
using AlpimiAPI.Settings;

namespace AlpimiAPI.Utilities
{
    public static class Configuration
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
        private static readonly string? _testConnectionString = Environment.GetEnvironmentVariable(
            "TEST_CONNECTION_STRING"
        );
        private static readonly string? _permitLimit = Environment.GetEnvironmentVariable(
            "PERMIT_LIMIT"
        );
        private static readonly string? _timeWindow = Environment.GetEnvironmentVariable(
            "TIME_WINDOW"
        );

        public const int perPage = PaginationSettings.perPage;
        public const int page = PaginationSettings.page;
        public const string sortBy = PaginationSettings.sortBy;
        public const string sortOrder = PaginationSettings.sortOrder;

        public static string? GetConnectionString()
        {
            return _connectionString;
        }

        public static string? GetTestConnectionString()
        {
            return _testConnectionString;
        }

        public static string GetJWTKey()
        {
            if (_jwtKey == null)
            {
                return AuthSettings.JWTKey;
            }
            return _jwtKey;
        }

        public static string GetJWTIssuer()
        {
            if (_jwtIssuer == null)
            {
                return AuthSettings.JWTIssuer;
            }
            return _jwtIssuer;
        }

        public static double GetJWTExpire()
        {
            if (_jwtExpire == null)
            {
                return AuthSettings.JWTExpire;
            }
            return Convert.ToDouble(_jwtExpire);
        }

        public static int GetHashIterations()
        {
            if (_hashIterations == null)
            {
                return AuthSettings.HashIterations;
            }
            return Convert.ToInt32(_hashIterations);
        }

        public static int GetKeySize()
        {
            if (_keySize == null)
            {
                return AuthSettings.KeySize;
            }
            return Convert.ToInt32(_keySize);
        }

        public static HashAlgorithmName GetHashAlgorithm()
        {
            if (_hashAlgorithm == null)
            {
                return new HashAlgorithmName(AuthSettings.HashAlgorithm);
            }
            return new HashAlgorithmName(_hashAlgorithm);
        }

        public static int GetPermitLimit()
        {
            if (_permitLimit == null)
            {
                return RateLimiterSettings.permitLimit;
            }
            return Convert.ToInt32(_permitLimit);
        }

        public static TimeSpan GetTimeWindow()
        {
            if (_timeWindow == null)
            {
                return RateLimiterSettings.timeWindow;
            }
            return TimeSpan.FromSeconds(Convert.ToInt32(_timeWindow));
        }
    }
}
