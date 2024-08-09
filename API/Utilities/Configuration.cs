namespace alpimi_planner_backend.API.Utilities
{
    public class Configuration
    {
        private static readonly string? _connectionString = Environment.GetEnvironmentVariable(
            "CONNECTION_STRING"
        );

        public static string? GetConnectionString()
        {
            return _connectionString;
        }
    }
}
