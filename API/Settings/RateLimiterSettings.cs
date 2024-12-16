namespace AlpimiAPI.Settings
{
    public static class RateLimiterSettings
    {
        public const int permitLimit = 40;
        public static TimeSpan timeWindow = TimeSpan.FromSeconds(5);
    }
}
