using System.Reflection;
using System.Text;
using AlpimiAPI;
using AlpimiAPI.Utilities;
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

    builder.Services.AddScoped<IDbService, DbService>();

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
        });

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Alpimi planner API", Version = "v1" });
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });

    var app = builder.Build();

    await AdminInit.StartupBase();

    //app.UseSwagger();
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

    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex.Message == "Connection String")
{
    Console.WriteLine("Cannot connect to the database. Is connection string correct?");
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}
catch (Exception)
{
    Console.WriteLine("Problem with Database");
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}
