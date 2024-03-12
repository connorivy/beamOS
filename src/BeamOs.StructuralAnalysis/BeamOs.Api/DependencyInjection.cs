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
        //IEnumerable<Type> endpointTypes = typeof(T)
        //    .Assembly
        //    .GetTypes()
        //    .Where(
        //        t => !t.IsAbstract && !t.IsInterface && t.IsAssignableTo(typeof(BeamOsEndpointBase))
        //    );

        //var scope = app.ServiceProvider.CreateScope();
        //foreach (Type type in endpointTypes)
        //{
        //    var endpoint = scope.ServiceProvider.GetService(type) as BeamOsEndpointBase;
        //    endpoint?.Map(app);
        //}

        _ = app.UseFastEndpoints(c =>
            {
                c.Endpoints.RoutePrefix = "api";
                c.Versioning.Prefix = "v";
                c.Endpoints.ShortNames = true;
                c.Endpoints.Filter = ed =>
                    ed.EndpointType.Assembly == typeof(IAssemblyMarkerApi).Assembly;
            })
            .UseSwaggerGen();
    }
}
