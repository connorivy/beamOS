using BeamOS.Common.Api;
using BeamOS.Common.Application;
using BeamOS.PhysicalModel.Api;
using BeamOS.PhysicalModel.Api.Common;
using BeamOS.PhysicalModel.Application;
using BeamOS.PhysicalModel.Infrastructure;
using FastEndpoints;
using FastEndpoints.ClientGen;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

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

builder.Services.AddMappers<IAssemblyMarkerPhysicalModelApi>();
builder.Services.AddBeamOsEndpoints<IAssemblyMarkerPhysicalModelApi>();
builder.Services.AddCommandHandlers<IAssemblyMarkerPhysicalModelApplication>();
builder.Services.AddPhysicalModelInfrastructure();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder
    .Services
    .AddDbContext<PhysicalModelDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGroup("/api").AddBeamOsEndpoints<IAssemblyMarkerPhysicalModelApi>();

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

const string clientNs = "BeamOS.PhysicalModel.Client";
await app.GenerateClientsAndExitAsync(
    alphaRelease,
    $"../{clientNs}/",
    csSettings: c =>
    {
        c.ClassName = "PhysicalModelAlphaClient";
        c.GenerateDtoTypes = false;

        const string beamOsNs = nameof(BeamOS);
        const string physicalModelNs = nameof(BeamOS.PhysicalModel);
        const string contractsNs = nameof(BeamOS.PhysicalModel.Contracts);
        const string nodeNs = nameof(BeamOS.PhysicalModel.Contracts.Node);
        const string element1dNs = nameof(BeamOS.PhysicalModel.Contracts.Element1D);
        const string modelNs = nameof(BeamOS.PhysicalModel.Contracts.Model);
        const string pointLoadNs = nameof(BeamOS.PhysicalModel.Contracts.PointLoad);

        const string contractsBaseNs = $"{beamOsNs}.{physicalModelNs}.{contractsNs}";
        c.AdditionalNamespaceUsages =
        [
            $"{contractsBaseNs}.{nodeNs}",
            $"{contractsBaseNs}.{element1dNs}",
            $"{contractsBaseNs}.{modelNs}",
            $"{contractsBaseNs}.{pointLoadNs}",
        ];
        c.GenerateClientInterfaces = true;
        c.CSharpGeneratorSettings.Namespace = clientNs;
        c.UseBaseUrl = false;
    },
    tsSettings: t =>
    {
        t.ClassName = "PhysicalModelAlphaClient";
        t.GenerateClientInterfaces = true;
        t.TypeScriptGeneratorSettings.Namespace = ""; // needed to not generate a namespace
    }
);

//Configure the HTTP-request pipeline
if (app.Environment.IsDevelopment())
{
    //seed the DB
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<PhysicalModelDbContext>();
    await dbContext.SeedAsync();
}

app.UseCors();

//SwaggerBuilderExtensions.UseSwagger(app);
//app.UseSwaggerUI();

app.Run();

namespace BeamOS.PhysicalModel.Clients.Cs { }
