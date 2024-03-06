using BeamOs.Api;
using BeamOS.Api;
using BeamOs.Api.Common;
using BeamOs.Application;
using BeamOs.Application.Common;
using BeamOs.Infrastructure;
using BeamOs.Infrastructure.PhysicalModel;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string alphaRelease = "Alpha Release";
builder
    .Services
    .AddFastEndpoints()
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
        //o.ExcludeNonFastEndpoints = true;
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

builder.Services.AddTransient<BeamOsFastEndpointOptions>();
builder.Services.AddMappers<IAssemblyMarkerApi>();
builder.Services.AddBeamOsEndpoints<IAssemblyMarkerApi>();
builder.Services.AddCommandHandlers<IAssemblyMarkerApplication>();
builder.Services.AddPhysicalModelInfrastructure();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder
    .Services
    .AddDbContext<BeamOsStructuralDbContext>(options => options.UseSqlServer(connectionString));
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
const string contractsBaseNs =
    $"{ApiClientGenerator.BeamOsNs}.{ApiClientGenerator.ContractsNs}.{ApiClientGenerator.PhysicalModelNs}";
await app.GenerateClient(
    alphaRelease,
    clientNs,
    clientName,

    [
        $"{contractsBaseNs}.{ApiClientGenerator.NodeNs}",
        $"{contractsBaseNs}.{ApiClientGenerator.Element1dNs}",
        $"{contractsBaseNs}.{ApiClientGenerator.ModelNs}",
        $"{contractsBaseNs}.{ApiClientGenerator.PointLoadNs}",
        $"{contractsBaseNs}.{ApiClientGenerator.MomentLoadNs}",
        $"{contractsBaseNs}.{ApiClientGenerator.MaterialNs}",
        $"{contractsBaseNs}.{ApiClientGenerator.SectionProfileNs}",
    ]
);

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

public partial class Program { }
