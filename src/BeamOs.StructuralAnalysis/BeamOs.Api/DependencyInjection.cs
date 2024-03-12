using BeamOs.Api.Common;
using BeamOs.Application;
using BeamOs.Application.Common;
using BeamOs.Infrastructure.PhysicalModel;
using FastEndpoints;
using FastEndpoints.Swagger;

namespace BeamOs.Api;

public static class DependencyInjection
{
    public static void AddAnalysisApiServices(this IServiceCollection services)
    {
        _ = services.AddTransient<BeamOsFastEndpointOptions>();
        _ = services.AddMappers<IAssemblyMarkerApi>();
        _ = services.AddBeamOsEndpoints<IAssemblyMarkerApi>();
        _ = services.AddCommandHandlers<IAssemblyMarkerApplication>();
        _ = services.AddPhysicalModelInfrastructure();
    }

    public static void AddBeamOsEndpointsForAnalysis(this IApplicationBuilder app)
    {
        const string alphaRelease = "Alpha Release";
        _ = app.UseFastEndpoints(c =>
            {
                c.Endpoints.RoutePrefix = "api";
                c.Versioning.Prefix = "v";
                c.Endpoints.ShortNames = true;
                c.Endpoints.Filter = ed =>
                    ed.EndpointType.Assembly == typeof(IAssemblyMarkerApi).Assembly;
            })
            .UseSwaggerGen(config =>
            {
                //config.DocumentName = alphaRelease;
                //config.Path = "/api";
                //config.
            });
    }
}
