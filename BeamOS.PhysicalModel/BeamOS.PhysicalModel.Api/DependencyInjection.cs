using BeamOS.PhysicalModel.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Api.Nodes.Endpoints;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOS.PhysicalModel.Api;
public static class DependencyInjection
{
    public static IServiceCollection AddPhysicalModelApi(this IServiceCollection services)
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
