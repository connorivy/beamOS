using System.Data.Common;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public class WebAppFactory(string connectionString)
    : WebApplicationFactory<IAssemblyMarkerStructuralAnalysisApi>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.Single(
                d => d.ServiceType == typeof(DbContextOptions<StructuralAnalysisDbContext>)
            );

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbConnection)
            );

            services.Remove(dbConnectionDescriptor);

            services.AddDbContext<StructuralAnalysisDbContext>(
                options => options.UseNpgsql(connectionString)
            //.UseModel(StructuralAnalysisDbContextModel.Instance)
            );

            using IServiceScope scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StructuralAnalysisDbContext>();
            dbContext.Database.EnsureCreated();
        });
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await AssemblySetup.TearDown();
    }
}

public class TestsInitializer
{
    public TestsInitializer() { }
}
