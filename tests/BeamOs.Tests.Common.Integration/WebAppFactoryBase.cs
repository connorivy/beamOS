using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

[RequiresDynamicCode("WebApplicationFactory uses reflection which is not compatible with AOT.")]
public class WebAppFactoryBase<TAssemblyMarker>(
    string connectionString,
    Action<IServiceCollection>? configureServices = null
) : WebApplicationFactory<TAssemblyMarker>
    where TAssemblyMarker : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("TEST_CONNECTION_STRING", connectionString);
        builder.ConfigureServices(services => configureServices?.Invoke(services));
    }
}
