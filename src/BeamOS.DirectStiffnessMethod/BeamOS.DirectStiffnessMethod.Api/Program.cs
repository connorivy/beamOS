using BeamOS.Common.Api;
using BeamOS.Common.Application;
using BeamOS.DirectStiffnessMethod.Api;
using BeamOS.DirectStiffnessMethod.Application;
using BeamOS.PhysicalModel.Client;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
const string alphaRelease = "Alpha Release";
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddMappers<IAssemblyMarkerDirectStiffnessMethodApi>();
builder.Services.AddBeamOsEndpoints<IAssemblyMarkerDirectStiffnessMethodApi>();
builder.Services.AddCommandHandlers<IAssemblyMarkerDirectStiffnessMethodApplication>();
builder
    .Services
    .AddHttpClient<IPhysicalModelAlphaClient, PhysicalModelAlphaClient>(
        client => client.BaseAddress = new("https://localhost:7193")
    );

builder
    .Services
    .AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("/api").AddBeamOsEndpoints<IAssemblyMarkerDirectStiffnessMethodApi>();

app.UseFastEndpoints(c =>
    {
        c.Endpoints.RoutePrefix = "api";
        c.Versioning.Prefix = "v";
        c.Endpoints.ShortNames = true;
    })
    .UseSwaggerGen();

const string clientNs = "BeamOS.DirectStiffnessMethod.Client";
const string clientName = "DirectStiffnessMethodAlphaClient";
const string contractsBaseNs =
    $"{ApiClientGenerator.BeamOsNs}.{nameof(BeamOS.DirectStiffnessMethod)}.{ApiClientGenerator.ContractsNs}";
await app.GenerateClient(
    alphaRelease,
    clientNs,
    clientName,
    [$"{contractsBaseNs}.{ApiClientGenerator.ModelNs}"]
);

app.UseCors();

app.Run();
