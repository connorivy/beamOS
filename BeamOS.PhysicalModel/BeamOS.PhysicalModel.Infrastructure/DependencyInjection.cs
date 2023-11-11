using BeamOS.PhysicalModel.Application.Common.Interfaces;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOS.PhysicalModel.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddPhysicalModelInfrastructure(this IServiceCollection services)
    {
        _ = services.AddSingleton<IRepository<ModelId, Model>, InMemoryRepository<ModelId, Model>>();
        _ = services.AddSingleton<IRepository<NodeId, Node>, InMemoryRepository<NodeId, Node>>();
        _ = services.AddSingleton<IRepository<Element1DId, Element1D>, InMemoryRepository<Element1DId, Element1D>>();
        _ = services.AddSingleton<IRepository<PointLoadId, PointLoad>, InMemoryRepository<PointLoadId, PointLoad>>();
        return services;
    }
}
