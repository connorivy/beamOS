using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Contracts.Common;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .ConfigureHttpJsonOptions(options =>
    {
        BeamOsSerializerOptions.DefaultConfig(options.SerializerOptions);
    });

builder
    .Services
    .AddStructuralAnalysisRequired()
    .AddStructuralAnalysisConfigurable(builder.Configuration.GetConnectionString("BeamOsDb"));

#if DEBUG
builder.Services.AddOpenApi();

builder
    .Services
    .AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
    });
#endif

MathNet.Numerics.Control.UseNativeMKL();

WebApplication app = builder.Build();

#if DEBUG
await app.InitializeBeamOsDb();
#endif

app.MapEndpoints<IAssemblyMarkerStructuralAnalysisApiEndpoints>();
app.UseCors();

#if DEBUG
app.MapOpenApi();
app.MapScalarApiReference();
#endif

app.Run();
