using AlpimiAPI.Locales;
using AlpimiAPI.Responses;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Localization;

namespace AlpimiAPI.Utilities
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddCustomRateLimiters(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter(
                    "Aggressive",
                    o =>
                    {
                        o.PermitLimit = Configuration.GetAggressivePermitLimit();
                        o.Window = Configuration.GetAggressiveTimeWindow();
                    }
                );

                options.AddFixedWindowLimiter(
                    "Strict",
                    o =>
                    {
                        o.PermitLimit = Configuration.GetStrictPermitLimit();
                        o.Window = Configuration.GetStrictTimeWindow();
                    }
                );

                options.AddFixedWindowLimiter(
                    "Regular",
                    o =>
                    {
                        o.PermitLimit = Configuration.GetRegularPermitLimit();
                        o.Window = Configuration.GetRegularTimeWindow();
                    }
                );

                options.AddFixedWindowLimiter(
                    "Moderate",
                    o =>
                    {
                        o.PermitLimit = Configuration.GetModeratePermitLimit();
                        o.Window = Configuration.GetModerateTimeWindow();
                    }
                );

                options.AddFixedWindowLimiter(
                    "Loose",
                    o =>
                    {
                        o.PermitLimit = Configuration.GetLoosePermitLimit();
                        o.Window = Configuration.GetLooseTimeWindow();
                    }
                );

                options.OnRejected = async (context, _) =>
                {
                    var _str = context.HttpContext.RequestServices.GetRequiredService<
                        IStringLocalizer<Errors>
                    >();

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "application/json";
                    var jsonResponse = System.Text.Json.JsonSerializer.Serialize(
                        new ApiErrorResponse(429, [new ErrorObject(_str["tooManyRequests"])])
                    );
                    await context.HttpContext.Response.WriteAsync(jsonResponse);
                };
            });

            return services;
        }
    }
}
