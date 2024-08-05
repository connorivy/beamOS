using System.Data.Common;
using BeamOs.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeamOs.Api.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (
            !bool.TryParse(
                Environment.GetEnvironmentVariable("ContinuousIntegrationBuild"),
                out bool isCiBuild
            ) || !isCiBuild
        )
        {
            base.ConfigureWebHost(builder);
            return;
        }

        // if this is a ci build, then replace the default connection string
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<BeamOsStructuralDbContext>)
            );

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbConnection)
            );

            services.Remove(dbConnectionDescriptor);

            const string connectionString =
                "Server=localhost,1433;Database=yourDatabaseName;Integrated Security=False;Encrypt=False;TrustServerCertificate=true;MultipleActiveResultSets=true;User=SA;Password=yourStrong(!)Password;";

            services.AddDbContext<BeamOsStructuralDbContext>(
                options => options.UseSqlServer(connectionString)
            );
        });
        builder.UseEnvironment("Release");
    }
}
