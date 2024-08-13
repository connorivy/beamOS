using BeamOs.Api;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    // event services
    .AddStructuralAnalysisApiEventServices()
    // structural analysis services
    .AddAnalysisEndpoints()
    .AddRequiredAnalysisEndpointServices()
    .AddAnalysisEndpointOptions()
    .AddAnalysisDb(builder.Configuration);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

app.AddAnalysisEndpoints();

app.UseHttpsRedirection();
app.UseCors();

// app.Configuration["generateclients"] = "true";
await app.GenerateAnalysisClient();

await app.InitializeAnalysisDb();

app.Run();

namespace BeamOs.Api
{
    public partial class Program { }
}
