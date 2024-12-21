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
    .AddStructuralAnalysisConfigurable(
        "Server=localhost;Port=5432;Database=some-postgres;Username=postgres;Password=mysecretpassword"
    );

#if DEBUG
builder.Services.AddOpenApi();
#endif

WebApplication app = builder.Build();

app.MapEndpoints();

#if DEBUG
app.MapOpenApi();
app.MapScalarApiReference();
#endif

app.Run();
