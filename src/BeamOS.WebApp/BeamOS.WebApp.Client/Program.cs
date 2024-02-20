using BeamOS.DirectStiffnessMethod.Client;
using BeamOS.PhysicalModel.Client;
using BeamOS.WebApp.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var client = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var response = await client.GetAsync("/app-settings");
builder.Configuration.AddJsonStream(response.Content.ReadAsStream());

var physicalModelUriString =
    builder.Configuration.GetValue<string>(Constants.PHYSICAL_MODEL_API_BASE_URI)
    ?? "https://localhost:7193";

var dsmUriString =
    builder.Configuration.GetValue<string>(Constants.DSM_API_BASE_URI) ?? "https://localhost:7110";

builder
    .Services
    .AddHttpClient<IPhysicalModelAlphaClient, PhysicalModelAlphaClient>(
        client => client.BaseAddress = new(physicalModelUriString)
    );

builder
    .Services
    .AddHttpClient<IDirectStiffnessMethodAlphaClient, DirectStiffnessMethodAlphaClient>(
        client => client.BaseAddress = new(dsmUriString)
    );

builder.Services.RegisterSharedServices();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();
