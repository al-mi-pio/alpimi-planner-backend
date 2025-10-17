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

        private static readonly string? _aggressivePermitLimit = Environment.GetEnvironmentVariable(
            "AGGRESSIVE_PERMIT_LIMIT"
        );
        private static readonly string? _aggressiveTimeWindow = Environment.GetEnvironmentVariable(
            "AGGRESSIVE_TIME_WINDOW"
        );

        private static readonly string? _strictPermitLimit = Environment.GetEnvironmentVariable(
            "STRICT_PERMIT_LIMIT"
        );
        private static readonly string? _strictTimeWindow = Environment.GetEnvironmentVariable(
            "STRICT_TIME_WINDOW"
        );

        private static readonly string? _regularPermitLimit = Environment.GetEnvironmentVariable(
            "REGULAR_PERMIT_LIMIT"
        );
        private static readonly string? _regularTimeWindow = Environment.GetEnvironmentVariable(
            "REGULAR_TIME_WINDOW"
        );

        private static readonly string? _moderatePermitLimit = Environment.GetEnvironmentVariable(
            "MODERATE_PERMIT_LIMIT"
        );
        private static readonly string? _moderateTimeWindow = Environment.GetEnvironmentVariable(
            "MODERATE_TIME_WINDOW"
        );

        private static readonly string? _loosePermitLimit = Environment.GetEnvironmentVariable(
            "LOOSE_PERMIT_LIMIT"
        );
        private static readonly string? _looseTimeWindow = Environment.GetEnvironmentVariable(
            "LOOSE_TIME_WINDOW"
        );

        private static readonly string? _historyLimit = Environment.GetEnvironmentVariable(
            "HISTORY_LIMIT"
        );

        public static int maxSchoolYearDuration { get; set; } = 24;
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

        public static int GetHistoryLimit()
        {
            if (_historyLimit == null)
            {
                return 10;
            }
            return Convert.ToInt32(_historyLimit);
        }

        public static RequiredCharacterTypes[]? GetRequiredCharacterTypesForPassword()
        {
            return CharacterSettings.RequiredCharacterTypesForPassword;
        }

        public static AllowedCharacterTypes[]? GetAllowedCharacterTypesForPassword()
        {
            return CharacterSettings.AllowedCharacterTypesForPassword;
        }

        public static AllowedCharacterTypes[]? GetAllowedCharacterTypesForLogin()
        {
            return CharacterSettings.AllowedCharacterTypesForLogin;
        }

        public static AllowedCharacterTypes[]? GetAllowedCharacterTypesForCustomURL()
        {
            return CharacterSettings.AllowedCharacterTypesForCustomURL;
        }

        public static AllowedCharacterTypes[]? GetAllowedCharacterTypesForScheduleName()
        {
            return CharacterSettings.AllowedCharacterTypesForScheduleName;
        }

        public static int GetAggressivePermitLimit()
        {
            if (_aggressivePermitLimit == null)
            {
                return RateLimiterSettings.aggressivePermitLimit;
            }
            return Convert.ToInt32(_aggressivePermitLimit);
        }

        public static TimeSpan GetAggressiveTimeWindow()
        {
            if (_aggressiveTimeWindow == null)
            {
                return RateLimiterSettings.aggressiveTimeWindow;
            }
            return TimeSpan.FromSeconds(Convert.ToInt32(_aggressiveTimeWindow));
        }

        public static int GetStrictPermitLimit()
        {
            if (_strictPermitLimit == null)
            {
                return RateLimiterSettings.strictPermitLimit;
            }
            return Convert.ToInt32(_strictPermitLimit);
        }

        public static TimeSpan GetStrictTimeWindow()
        {
            if (_strictTimeWindow == null)
            {
                return RateLimiterSettings.strictTimeWindow;
            }
            return TimeSpan.FromSeconds(Convert.ToInt32(_strictTimeWindow));
        }

        public static int GetRegularPermitLimit()
        {
            if (_regularPermitLimit == null)
            {
                return RateLimiterSettings.regularPermitLimit;
            }
            return Convert.ToInt32(_regularPermitLimit);
        }

        public static TimeSpan GetRegularTimeWindow()
        {
            if (_regularTimeWindow == null)
            {
                return RateLimiterSettings.regularTimeWindow;
            }
            return TimeSpan.FromSeconds(Convert.ToInt32(_regularTimeWindow));
        }

        public static int GetModeratePermitLimit()
        {
            if (_moderatePermitLimit == null)
            {
                return RateLimiterSettings.moderatePermitLimit;
            }
            return Convert.ToInt32(_moderatePermitLimit);
        }

        public static TimeSpan GetModerateTimeWindow()
        {
            if (_moderateTimeWindow == null)
            {
                return RateLimiterSettings.moderateTimeWindow;
            }
            return TimeSpan.FromSeconds(Convert.ToInt32(_moderateTimeWindow));
        }

        public static int GetLoosePermitLimit()
        {
            if (_loosePermitLimit == null)
            {
                return RateLimiterSettings.loosePermitLimit;
            }
            return Convert.ToInt32(_loosePermitLimit);
        }

        public static TimeSpan GetLooseTimeWindow()
        {
            if (_looseTimeWindow == null)
            {
                return RateLimiterSettings.looseTimeWindow;
            }
            return TimeSpan.FromSeconds(Convert.ToInt32(_looseTimeWindow));
        }
    }
}
