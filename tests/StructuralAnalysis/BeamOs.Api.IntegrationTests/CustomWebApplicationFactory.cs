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
    //protected override void ConfigureWebHost(IWebHostBuilder builder)
    //{
    //    builder.ConfigureServices(services =>
    //    {
    //        var dbContextDescriptor = services.SingleOrDefault(
    //            d => d.ServiceType == typeof(DbContextOptions<BeamOsStructuralDbContext>)
    //        );

    //        services.Remove(dbContextDescriptor);

    //        var dbConnectionDescriptor = services.SingleOrDefault(
    //            d => d.ServiceType == typeof(DbConnection)
    //        );

    //        services.Remove(dbConnectionDescriptor);

    //        // Create open SqliteConnection so EF won't automatically close it.
    //        services.AddSingleton<DbConnection>(container =>
    //        {
    //            var connection = new SqliteConnection("DataSource=:memory:");
    //            connection.Open();

    //            return connection;
    //        });

    //        services.AddDbContext<BeamOsStructuralDbContext>(
    //            (container, options) =>
    //            {
    //                var connection = container.GetRequiredService<DbConnection>();
    //                options.UseSqlite(connection);
    //            }
    //        );
    //    });

    //    builder.UseEnvironment("Development");
    //}

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

        builder.UseEnvironment("Development");
    }
}
