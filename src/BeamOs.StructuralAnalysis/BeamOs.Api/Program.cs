using BeamOs.Api;
using BeamOs.Api.Common;
using BeamOs.Infrastructure;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string alphaRelease = "Alpha Release";
builder
    .Services
    .AddFastEndpoints(options =>
    {
        options.DisableAutoDiscovery = true;
        options.Assemblies =  [typeof(IAssemblyMarkerApi).Assembly];
    })
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.DocumentName = alphaRelease;
            s.Title = "beamOS api";
            s.Version = "v0";
            s.SchemaSettings.SchemaProcessors.Add(new MarkAsRequiredIfNonNullableSchemaProcessor());
        };
        o.ShortSchemaNames = true;
        o.ExcludeNonFastEndpoints = true;
    });

builder
    .Services
    .AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
    });

builder.Services.AddAnalysisApiServices();

var connectionString =
    builder.Configuration.GetConnectionString("AnalysisDbConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder
    .Services
    .AddDbContext<BeamOsStructuralDbContext>(options => options.UseSqlServer(connectionString))
    .AddPhysicalModelInfrastructureReadModel(connectionString);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

app.UseHttpsRedirection();

//app.MapGroup("/api").AddBeamOsEndpoints<IAssemblyMarkerApi>();

app.UseFastEndpoints(c =>
    {
        c.Endpoints.RoutePrefix = "api";
        c.Versioning.Prefix = "v";
        c.Endpoints.ShortNames = true;
    })
    .UseSwaggerGen();

// using NSwag to generate cs and ts api clients.
// I tried with kiota, but kiota does not allow providing your own DTOs and the generated DTOs
// had all values as nullable with no constructors 🤮

const string clientNs = "BeamOs.ApiClient";
const string clientName = "ApiAlphaClient";
const string contractsBaseNs = $"{ApiClientGenerator.BeamOsNs}.{ApiClientGenerator.ContractsNs}";
const string physicalModelBaseNs = $"{contractsBaseNs}.{ApiClientGenerator.PhysicalModelNs}";
const string analyticalResultsBaseNs =
    $"{contractsBaseNs}.{ApiClientGenerator.AnalyticalResultsNs}";

await app.GenerateClient(alphaRelease, clientNs, clientName);

//app.MapGet("/user", (ClaimsPrincipal user) => $"Hello user {user.Identity.Name}")
//.RequireAuthorization();

//Configure the HTTP-request pipeline
if (app.Environment.IsDevelopment())
{
    //seed the DB
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BeamOsStructuralDbContext>();
    await dbContext.SeedAsync();
}

app.UseCors();

//SwaggerBuilderExtensions.UseSwagger(app);
//app.UseSwaggerUI();

app.Run();

namespace BeamOs.Api
{
    public partial class Program { }
}
