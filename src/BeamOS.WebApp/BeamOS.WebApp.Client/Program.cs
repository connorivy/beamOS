//using BeamOS.PhysicalModel.Clients.Cs;
//using BeamOS.PhysicalModel.Clients.Cs.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

//builder
//    .Services
//    .AddHttpClient<PhysicalModelAlphaClientFactory>(
//        client => client.BaseAddress = new("https://localhost:7193")
//    );

//builder
//    .Services
//    .AddScoped<PhysicalModelAlphaClient>(
//        x => x.GetRequiredService<PhysicalModelAlphaClientFactory>().Create()
//    );

await builder.Build().RunAsync();
