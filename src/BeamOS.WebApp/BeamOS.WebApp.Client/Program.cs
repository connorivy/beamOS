using BeamOS.PhysicalModel.Client;
using BeamOS.WebApp.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var client = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var response = await client.GetAsync("/app-settings");
builder.Configuration.AddJsonStream(response.Content.ReadAsStream());

var uriString =
    builder.Configuration.GetValue<string>("PhysicalModelApiBaseUriString")
    ?? "https://localhost:7193";

builder
    .Services
    .AddHttpClient<IPhysicalModelAlphaClient, PhysicalModelAlphaClient>(
        client => client.BaseAddress = new(uriString)
    );

builder.Services.AddScoped<IPhysicalModelAlphaClientWithEditor, PhysicalModelAlphaClientProxy>();

await builder.Build().RunAsync();
