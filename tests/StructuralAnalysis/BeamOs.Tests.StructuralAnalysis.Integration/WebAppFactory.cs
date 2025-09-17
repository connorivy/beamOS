#if !RUNTIME
using System.Diagnostics.CodeAnalysis;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

[RequiresDynamicCode("WebApplicationFactory uses reflection which is not compatible with AOT.")]
public sealed class WebAppFactory(string connectionString, TimeProvider? timeProvider = null)
    : WebApplicationFactory<IAssemblyMarkerStructuralAnalysisApi>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("TEST_CONNECTION_STRING", connectionString);
        builder.ConfigureServices(services =>
        {
            using IServiceScope scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StructuralAnalysisDbContext>();
            dbContext.Database.EnsureCreated();
            // dbContext.Database.Migrate();
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

#endif
