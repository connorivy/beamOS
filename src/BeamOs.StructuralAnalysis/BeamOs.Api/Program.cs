using BeamOs.Api;
using BeamOs.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddAnalysisEndpoints()
    .AddAnalysisEndpointServices()
    .AddAnalysisEndpointConfigurableServices()
    .AddAnalysisEndpointOptions()
    .AddAnalysisInfrastructure(builder.Configuration);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

app.UseHttpsRedirection();

app.AddAnalysisEndpoints();

// app.Configuration["generateclients"] = "true";
await app.GenerateAnalysisClient();

//Configure the HTTP-request pipeline
if (app.Environment.IsDevelopment())
{
    //seed the DB
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BeamOsStructuralDbContext>();
    await dbContext.SeedAsync();
}

app.UseCors();

app.Run();

namespace BeamOs.Api
{
    public partial class Program { }
}
