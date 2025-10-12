using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

[RequiresDynamicCode("WebApplicationFactory uses reflection which is not compatible with AOT.")]
public class ExternalWebAppFactory<TAssemblyMarker> : WebApplicationFactory<TAssemblyMarker>
    where TAssemblyMarker : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Configure the Kestrel server to listen on a specific port
        builder.ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder.UseKestrel().UseUrls("https://127.0.0.1:5000"); // Replace with your desired port
        });

        return base.CreateHost(builder);
    }
}
