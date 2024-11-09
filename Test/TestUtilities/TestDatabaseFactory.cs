using System.Data;
using System.Data.Common;
using AlpimiAPI.Database;
using AlpimiAPI.Entities.EAuth;
using AlpimiAPI.Entities.EUser;
using AlpimiAPI.Utilities;
using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AlpimiTest.TestUtilities
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
        where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(GetProjectContentRoot());
            builder.ConfigureServices(
                (context, services) =>
                {
                    services.AddScoped<IDbConnection>(sp => new SqlConnection(
                        "Data Source=DESKTOP-F50MD76;Initial Catalog=testy2;Integrated Security=True;Pooling=False;Encrypt=True;Trust Server Certificate=True"
                    ));
                }
            );
        }

        private string GetProjectContentRoot()
        {
            var projectDir = Directory.GetCurrentDirectory();
            return Path.Combine(projectDir, "C:/Users/Piotr/source/repos/alpimi-planner-backend");
        }
    }
}
