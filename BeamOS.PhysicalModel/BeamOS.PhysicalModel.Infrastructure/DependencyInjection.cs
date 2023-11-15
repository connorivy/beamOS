using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOS.PhysicalModel.Infrastructure;
public static class DependencyInjection
{
    public static void AddPhysicalModelInfrastructure(this ModelConfigurationBuilder configurationBuilder)
    {
        IEnumerable<Type> valueConverters = typeof(DependencyInjection).Assembly
            .GetTypes()
            .Where(t => t.IsClass
                && t.BaseType is not null
                && t.BaseType.IsGenericType
                && t.BaseType.GetGenericTypeDefinition() == typeof(ValueConverter<,>));

        foreach (var valueConverterType in valueConverters)
        {
            var genericArgs = valueConverterType.BaseType?.GetGenericArguments();
            if (genericArgs?.Length != 2)
            {
                throw new ArgumentException();
            }

            _ = configurationBuilder
                .Properties(genericArgs[0])
                .HaveConversion(valueConverterType);
        }
    }

    public static IServiceCollection AddPhysicalModelInfrastructure(this IServiceCollection services)
    {
        _ = services.AddSingleton<IRepository<ModelId, Model>, InMemoryRepository<ModelId, Model>>();
        _ = services.AddSingleton<IRepository<NodeId, Node>, InMemoryRepository<NodeId, Node>>();
        _ = services.AddSingleton<IRepository<Element1DId, Element1D>, InMemoryRepository<Element1DId, Element1D>>();
        _ = services.AddSingleton<IRepository<PointLoadId, PointLoad>, InMemoryRepository<PointLoadId, PointLoad>>();
        return services;
    }
}
