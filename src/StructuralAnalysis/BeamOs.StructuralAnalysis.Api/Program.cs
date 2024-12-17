using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Application;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    BeamOsSerializerOptions.DefaultConfig(options.SerializerOptions);
});

builder.Services.AddStructuralAnalysis(
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
