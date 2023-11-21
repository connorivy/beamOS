using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Api.Nodes.Endpoints;

namespace BeamOS.PhysicalModel.Api;
public static class DependencyInjection
{
    public static IServiceCollection AddMappers(this IServiceCollection services)
    {
        _ = services.Scan(scan => scan
            .FromAssemblyOf<CreateNodeEndpoint>()
            .AddClasses(classes => classes.AssignableTo(typeof(IMapper<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime()
        );

        return services;
    }
}
