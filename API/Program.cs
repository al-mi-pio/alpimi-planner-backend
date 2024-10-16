using System.Text;
using AlpimiAPI.Auth;
using alpimi_planner_backend.API;
using alpimi_planner_backend.API.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

builder.Services.AddScoped<IDbService, DbService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
var app = builder.Build();

//app.UseSwagger();
app.UseSwagger(c =>
{
    c.RouteTemplate = "api/{documentname}/swagger.json";
});

//app.UseSwaggerUI();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/api/v1/swagger.json", "API V1");
    c.RoutePrefix = "api";
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
