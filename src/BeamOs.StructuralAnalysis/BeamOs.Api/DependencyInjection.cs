using BeamOs.Api.Common;
using BeamOs.Api.Common.Extensions;
using BeamOs.Application;
using BeamOs.Application.Common;
using BeamOs.Infrastructure;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Api;

public static class DependencyInjection
{
    const string alphaRelease = "Alpha Release";

    public static IServiceCollection AddAnalysisEndpoints(this IServiceCollection services)
    {
        services
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
                    s.SchemaSettings
                        .SchemaProcessors
                        .Add(new MarkAsRequiredIfNonNullableSchemaProcessor());
                };
                o.ShortSchemaNames = true;
                o.ExcludeNonFastEndpoints = true;
            });
        return services;
    }

    public static IServiceCollection AddAnalysisEndpointOptions(this IServiceCollection services)
    {
        _ = services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });

        _ = services.AddTransient<BeamOsFastEndpointOptions>();
        return services;
    }

    public static IServiceCollection AddAnalysisEndpointServices(this IServiceCollection services)
    {
        _ = services.AddMappers<IAssemblyMarkerApi>();
        //_ = services.AddBeamOsEndpoints<IAssemblyMarkerApi>();
        _ = services.AddCommandHandlers<IAssemblyMarkerApplication>();

        return services;
    }

    public static IServiceCollection AddAnalysisEndpointConfigurableServices(
        this IServiceCollection services
    )
    {
        return services.AddScoped<IntegrationEvents.IEventBus, DummyEventBus>();
    }

    public static IServiceCollection AddAnalysisInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString =
            configuration.GetConnectionString("AnalysisDbConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found."
            );
        services
            .AddDbContext<BeamOsStructuralDbContext>(
                options => options.UseSqlServer(connectionString)
            )
            .AddPhysicalModelInfrastructureReadModel(connectionString);

        services.AddPhysicalModelInfrastructure();

        return services;
    }

    public static void AddAnalysisEndpoints(this IApplicationBuilder app)
    {
        _ = app.UseFastEndpoints(c =>
            {
                c.Endpoints.RoutePrefix = "api";
                c.Versioning.Prefix = "v";
                c.Endpoints.ShortNames = true;
                c.Endpoints.Filter = ed =>
                    ed.EndpointType.Assembly == typeof(IAssemblyMarkerApi).Assembly;
                c.Serializer.Options.IgnoreRequiredKeyword();
            })
            .UseSwaggerGen();
    }

    public static async Task GenerateAnalysisClient(this WebApplication app)
    {
        const string clientNs = "BeamOs.ApiClient";
        const string clientName = "ApiAlphaClient";
        const string contractsBaseNs =
            $"{ApiClientGenerator.BeamOsNs}.{ApiClientGenerator.ContractsNs}";
        const string physicalModelBaseNs =
            $"{contractsBaseNs}.{ApiClientGenerator.PhysicalModelNs}";
        const string analyticalResultsBaseNs =
            $"{contractsBaseNs}.{ApiClientGenerator.AnalyticalResultsNs}";

        await app.GenerateClient(alphaRelease, clientNs, clientName);
    }
}
