using BeamOs.Application.PhysicalModel.Element1dAggregate;
using BeamOs.Application.PhysicalModel.Models.Commands;
using BeamOs.Application.PhysicalModel.Nodes.Commands;
using BeamOs.Application.PhysicalModel.PointLoads.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Application.PhysicalModel;

public static class DependencyInjection
{
    public static IServiceCollection AddPhysicalModelApplication(this IServiceCollection services)
    {
        _ = services.AddTransient<CreateModelCommandHandler>();
        _ = services.AddTransient<CreateNodeCommandHandler>();
        _ = services.AddTransient<CreateElement1dCommandHandler>();
        _ = services.AddTransient<CreatePointLoadCommandHandler>();

        _ = services.AddTransient<GetModelCommandHandler>();

        return services;
    }
}
