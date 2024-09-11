using BeamOs.Api;
using BeamOS.WebApp;
using BeamOs.WebApp.Client.Components;

var builder = WebApplication.CreateBuilder(args);

// metrics
builder
    .Services
    .AddMetrics()
    .AddSingleton<StructuralAnalysisMetrics>()
    .AddOpenTelemetry()
    .WithMetrics(metrics => metrics.AddMeter(StructuralAnalysisMetrics.MeterName));

builder
    .Services
    // event services
    .AddEventServices()
    // identity services
    .AddIdentityServices(builder.Configuration)
    // structural analysis services
    .AddAnalysisEndpoints()
    .AddRequiredAnalysisEndpointServices()
    .AddAnalysisEndpointOptions()
    .AddAnalysisDb(builder.Configuration)
    // webapp services
    .AddRequiredWebServerServices()
    .AddConfigurableWebServerServices(builder.Configuration)
    // client services
    .RegisterSharedServices<Program>();

var app = builder.Build();

// structural analysis config
app.AddAnalysisEndpoints();
await app.InitializeAnalysisDb();

// webapp config
app.RequiredWebApplicationConfig();
app.ConfigurableWebApplicationConfig();

app.Run();

namespace BeamOS.WebApp
{
    public partial class Program { }
}
