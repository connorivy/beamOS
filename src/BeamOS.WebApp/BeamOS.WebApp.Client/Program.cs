using BeamOS.PhysicalModel.Client.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder
    .Services
    .AddRefitClient<IPhysicalModelAlphaClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7193/"));

await builder.Build().RunAsync();
