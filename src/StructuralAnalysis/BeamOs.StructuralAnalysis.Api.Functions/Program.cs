using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Infrastructure;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

string connectionString =
    "Server=localhost;Port=5432;Database=some-postgres;Username=postgres;Password=mysecretpassword";
bool isTestContainer = false;

#if DEBUG
System.Diagnostics.Debugger.Launch();
#endif

if (Environment.GetEnvironmentVariable("TEST_CONNECTION_STRING") is string cnxString)
{
    connectionString = cnxString;
    isTestContainer = true;
}

var builder = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            BeamOsSerializerOptions.DefaultConfig(options.SerializerOptions);
        });
        services.AddStructuralAnalysis(connectionString);
    });

var app = builder.Build();

if (isTestContainer)
{
    using IServiceScope scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<StructuralAnalysisDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

app.Run();
