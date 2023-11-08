using BeamOS.PhysicalModel.Application.Models.Commands;
using BeamOS.PhysicalModel.Application.Nodes.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOS.PhysicalModel.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddPhysicalModelApplication(this IServiceCollection services)
    {
        _ = services.AddTransient<CreateModelCommandHandler>();
        _ = services.AddTransient<CreateNodeCommandHandler>();
        return services;
    }
}
