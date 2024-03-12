using System.Data.Common;
using BeamOs.Infrastructure;
using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod;

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
}

public class CustomApp : AppFixture<BeamOS.WebApp.Program>
{
    //protected override void ConfigureApp(IWebHostBuilder a)
    //{
    //    //only needed when tests are not in a separate project
    //    //a.UseContentRoot(Directory.GetCurrentDirectory());
    //}

    //protected override void ConfigureServices(IServiceCollection s)
    //{
    //    //s.AddSingleton<IAmazonSimpleEmailServiceV2, FakeSesClient>();
    //}
}
