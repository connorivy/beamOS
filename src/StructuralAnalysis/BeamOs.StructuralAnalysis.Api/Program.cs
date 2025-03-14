using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.SpeckleConnector;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.Tests.Common;
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

builder
    .Services
    .AddObjectThatExtendsBase<IAssemblyMarkerSpeckleConnectorApi>(
        typeof(BeamOsBaseEndpoint<,>),
        ServiceLifetime.Scoped
    );

#if DEBUG
builder
    .Services
    .AddOpenApi(o =>
    {
        //o.AddSchemaTransformer<EnumSchemaTransformer>();
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
#endif

if (!BeamOsEnv.IsCiEnv())
{
    MathNet.Numerics.Control.UseNativeMKL();
}

WebApplication app = builder.Build();

#if DEBUG
await app.InitializeBeamOsDb();
#endif

app.MapEndpoints<IAssemblyMarkerStructuralAnalysisApiEndpoints>();
app.MapEndpoints<IAssemblyMarkerSpeckleConnectorApi>();

#if DEBUG
app.UseCors();
app.MapOpenApi();
app.MapScalarApiReference();
#endif

app.Run();
