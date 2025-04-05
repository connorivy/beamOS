using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.SpeckleConnector;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.Tests.Common;
using Microsoft.AspNetCore.Http.Metadata;
using Objects.BuiltElements;
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

builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));

#if DEBUG
builder
    .Services
    .AddOpenApi(o =>
    {
        //o.AddSchemaTransformer<EnumSchemaTransformer>();
    })
    .AddOpenApi(
        "ai",
        options =>
        {
            options.ShouldInclude = (description) =>
            {
                foreach (var data in description.ActionDescriptor.EndpointMetadata)
                {
                    if (data is ITagsMetadata tagsMetadata)
                    {
                        if (tagsMetadata.Tags.Contains(BeamOsTags.AI))
                        {
                            return true;
                        }
                    }
                }
                return false;
            };
        }
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
#endif

// if (!BeamOsEnv.IsCiEnv())
// {
//     MathNet.Numerics.Control.UseNativeMKL();
// }

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
