namespace AlpimiAPI.Settings
{
    public static class RateLimiterSettings
    {
        public const int permitLimit = 30;
        public static TimeSpan timeWindow = TimeSpan.FromSeconds(5);
    }
}
