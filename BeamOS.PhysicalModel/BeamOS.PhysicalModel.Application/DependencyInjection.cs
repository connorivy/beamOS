using BeamOS.PhysicalModel.Application.Element1Ds;
using BeamOS.PhysicalModel.Application.Models.Commands;
using BeamOS.PhysicalModel.Application.Nodes.Commands;
using BeamOS.PhysicalModel.Application.PointLoads.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOS.PhysicalModel.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddPhysicalModelApplication(this IServiceCollection services)
    {
        _ = services.AddTransient<CreateModelCommandHandler>();
        _ = services.AddTransient<CreateNodeCommandHandler>();
        _ = services.AddTransient<CreateElement1DCommandHandler>();
        _ = services.AddTransient<CreatePointLoadCommandHandler>();
        return services;
    }
}
