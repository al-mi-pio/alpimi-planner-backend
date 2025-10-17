namespace AlpimiAPI.Settings
{
    public static class RateLimiterSettings
    {
        public const int aggressivePermitLimit = 10;
        public static TimeSpan aggressiveTimeWindow = TimeSpan.FromSeconds(600);

        public const int strictPermitLimit = 10;
        public static TimeSpan strictTimeWindow = TimeSpan.FromSeconds(10);

        public const int regularPermitLimit = 20;
        public static TimeSpan regularTimeWindow = TimeSpan.FromSeconds(5);

        public const int moderatePermitLimit = 50;
        public static TimeSpan moderateTimeWindow = TimeSpan.FromSeconds(5);

        public const int loosePermitLimit = 100;
        public static TimeSpan looseTimeWindow = TimeSpan.FromSeconds(5);
    }
}
