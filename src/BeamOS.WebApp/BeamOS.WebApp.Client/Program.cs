//using BeamOS.PhysicalModel.Clients.Cs;
//using BeamOS.PhysicalModel.Clients.Cs.Extensions;
using BeamOS.PhysicalModel.Client;
using BeamOS.WebApp.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder
    .Services
    .AddHttpClient<IPhysicalModelAlphaClient, PhysicalModelAlphaClient>(
        client => client.BaseAddress = new("https://localhost:7193")
    );

builder.Services.AddScoped<IPhysicalModelAlphaClientWithEditor, PhysicalModelAlphaClientProxy>();

await builder.Build().RunAsync();
