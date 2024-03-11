//using BeamOs.ApiClient;
using BeamOS.DirectStiffnessMethod.Client;
using BeamOS.PhysicalModel.Client;
using BeamOS.WebApp.Client;
using Blazored.LocalStorage;
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

var analysisUriString =
    builder.Configuration.GetValue<string>(Constants.DSM_API_BASE_URI) ?? "https://localhost:7111";

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

//builder
//    .Services
//    .AddHttpClient<IApiAlphaClient, ApiAlphaClient>(
//        client => client.BaseAddress = new(analysisUriString)
//    );

builder.Services.RegisterSharedServices();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddBlazoredLocalStorage();
builder
    .Services
    .AddSingleton<Func<ILocalStorageService>>(
        sp =>
            () =>
            {
                var scope = sp.CreateScope();
                return scope.ServiceProvider.GetRequiredService<ILocalStorageService>();
            }
    );

await builder.Build().RunAsync();
