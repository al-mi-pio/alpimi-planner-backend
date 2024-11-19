using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using AlpimiAPI;
using AlpimiAPI.Database;
using AlpimiAPI.Responses;
using AlpimiAPI.Settings;
using AlpimiAPI.Utilities;
using alpimi_planner_backend.API.Locales;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

try
{
    builder.Services.AddControllers();
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly)
    );

    builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(
        Configuration.GetConnectionString()
    ));

    builder.Services.AddScoped<IDbService, DbService>();

    builder.Services.AddLocalization();

    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("pl-PL")
        };
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Authentication token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                }
            );
            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new String[] { }
                    }
                }
            );
        });
    }
    builder
        .Services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = "Bearer";
            option.DefaultScheme = "Bearer";
            option.DefaultChallengeScheme = "Bearer";
        })
        .AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = Configuration.GetJWTIssuer(),
                ValidAudience = Configuration.GetJWTIssuer(),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration.GetJWTKey())
                ),
            };
            cfg.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    var _str = context.HttpContext.RequestServices.GetRequiredService<
                        IStringLocalizer<Errors>
                    >();

                    context.HandleResponse();

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var jsonResponse = System.Text.Json.JsonSerializer.Serialize(
                        new ApiErrorResponse(401, [new ErrorObject(_str["notAuthenticated"])])
                    );

                    return context.Response.WriteAsync(jsonResponse);
                },
                OnForbidden = context =>
                {
                    var _str = context.HttpContext.RequestServices.GetRequiredService<
                        IStringLocalizer<Errors>
                    >();

                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    var jsonResponse = System.Text.Json.JsonSerializer.Serialize(
                        new ApiErrorResponse(403, [new ErrorObject(_str["notAuthorized"])])
                    );

                    return context.Response.WriteAsync(jsonResponse);
                }
            };
        });

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Alpimi planner API", Version = "v1" });
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });
    builder
        .Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context
                    .ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return new BadRequestObjectResult(
                    new ApiErrorResponse(400, [new ErrorObject(errors[0])!])
                );
            };
        });

    builder.Services.AddRateLimiter(options =>
    {
        options
            .AddFixedWindowLimiter(
                "FixedWindow",
                limiterOptions =>
                {
                    limiterOptions.PermitLimit = RateLimiterSettings.permitLimit;
                    limiterOptions.Window = RateLimiterSettings.timeWindow;
                }
            )
            .OnRejected = async (context, _) =>
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

    var app = builder.Build();
    var adminInit = new AdminInit(app.Services.GetService<IStringLocalizer<General>>()!);

    if (!app.Environment.IsEnvironment("Testing"))
    {
        await adminInit.StartupBase();
    }

    app.UseRateLimiter();

    app.UseSwagger(c =>
    {
        c.RouteTemplate = "api/{documentname}/swagger.json";
    });

    //app.UseSwaggerUI();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api/v1/swagger.json", "API V1");
        if (app.Environment.IsProduction())
        {
            c.SupportedSubmitMethods();
        }
        c.RoutePrefix = "api";
    });

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseRequestLocalization();

    app.MapControllers();

    app.Run();
}
catch (ApiErrorException ex)
{
    Console.WriteLine(ex.errors.FirstOrDefault()!.message);
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}
catch (Exception)
{
    Console.WriteLine("Problem with Database");
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}
