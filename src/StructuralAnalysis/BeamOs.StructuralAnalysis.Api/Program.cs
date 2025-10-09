using BeamOs.Ai;
using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.SpeckleConnector;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Infrastructure;
using BeamOs.StructuralAnalysis.Sdk;
using Microsoft.AspNetCore.Http.Metadata;
using Scalar.AspNetCore;
using Testcontainers.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

string connectionString;
bool isTestContainer;
if (args[0] == "USE_TEMP_DB")
{
    PostgreSqlContainer testContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();
    await testContainer.StartAsync();
    connectionString = $"{testContainer.GetConnectionString()};Include Error Detail=True";
    isTestContainer = true;
}
else if (
    Environment.GetEnvironmentVariable("TEST_CONNECTION_STRING") is string testConnectionString
)
{
    connectionString = testConnectionString;
    isTestContainer = true;
}
else
{
    connectionString =
        builder.Configuration.GetConnectionString("BeamOsDb")
        ?? throw new InvalidOperationException("Connection string 'BeamOsDb' not found.");
    isTestContainer = false;
}

builder.Services.ConfigureHttpJsonOptions(options =>
{
    BeamOsSerializerOptions.DefaultConfig(options.SerializerOptions);
});

builder
    .Services.AddStructuralAnalysisRequired()
    .AddStructuralAnalysisConfigurable()
    .AddStructuralAnalysisInfrastructureRequired()
    .AddStructuralAnalysisInfrastructureConfigurable(connectionString);

builder.Services.AddObjectThatExtendsBase<IAssemblyMarkerAi>(
    typeof(BeamOsBaseEndpoint<,>),
    ServiceLifetime.Scoped
);
builder.Services.AddObjectThatExtendsBase<IAssemblyMarkerStructuralAnalysisApiEndpoints>(
    typeof(BeamOsBaseEndpoint<,>),
    ServiceLifetime.Scoped
);

builder.Services.AddLogging(b =>
{
    b.ClearProviders();
#if DEBUG
    b.AddConsole().SetMinimumLevel(LogLevel.Trace);
#else
    b.AddConsole().SetMinimumLevel(LogLevel.Error);
#endif
});

#if DEBUG
builder
    .Services.AddOpenApi(o =>
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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
#endif

builder.Services.AddAi();

// if (!BeamOsEnv.IsCiEnv())
// {
//     MathNet.Numerics.Control.UseNativeMKL();
// }

WebApplication app = builder.Build();

// #if DEBUG
if (!isTestContainer)
{
    await app.InitializeBeamOsDb();
}

// #endif

// app.MapStructuralEndpoints();
// app.MapSpeckleEndpoints();
app.MapEndpoints<IAssemblyMarkerStructuralAnalysisApiEndpoints>();
app.MapEndpoints<IAssemblyMarkerSpeckleConnector>();
app.MapEndpoints<IAssemblyMarkerAi>();

#if DEBUG
app.UseCors();
app.MapOpenApi();
app.MapScalarApiReference();
#endif

app.Run();
