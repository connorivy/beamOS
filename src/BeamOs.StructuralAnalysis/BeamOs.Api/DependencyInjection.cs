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
    public const string AlphaRelease = "Alpha Release";

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
                    s.DocumentName = AlphaRelease;
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

    public static IServiceCollection AddRequiredAnalysisEndpointServices(
        this IServiceCollection services
    )
    {
        return services
            .AddMappers<IAssemblyMarkerApi>()
            .AddCommandHandlers<IAssemblyMarkerApplication>()
            .AddPhysicalModelInfrastructure();
    }

    public static IServiceCollection AddStructuralAnalysisApiEventServices(
        this IServiceCollection services
    )
    {
        services.AddMediatR(
            config => config.RegisterServicesFromAssembly(typeof(IAssemblyMarkerApi).Assembly)
        );
        return services.AddScoped<IntegrationEvents.IEventBus, DummyEventBus>();
    }

    public static IServiceCollection AddAnalysisDb(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString =
            configuration.GetConnectionString("AnalysisDbConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found."
            );
        services.AddDbContext<BeamOsStructuralDbContext>(
            options => options.UseSqlServer(connectionString)
        );

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

    public static async Task InitializeAnalysisDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeamOsStructuralDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        await dbContext.SeedAsync();
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

        await app.GenerateClient(AlphaRelease, clientNs, clientName);
    }
}
