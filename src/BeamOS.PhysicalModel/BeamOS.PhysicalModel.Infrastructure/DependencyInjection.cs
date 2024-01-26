using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.MaterialAggregate;
using BeamOS.PhysicalModel.Domain.MaterialAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate.ValueObjects;
using BeamOS.PhysicalModel.Infrastructure.Element1Ds;
using BeamOS.PhysicalModel.Infrastructure.Materials;
using BeamOS.PhysicalModel.Infrastructure.Models;
using BeamOS.PhysicalModel.Infrastructure.MomentLoads;
using BeamOS.PhysicalModel.Infrastructure.Nodes;
using BeamOS.PhysicalModel.Infrastructure.PointLoads;
using BeamOS.PhysicalModel.Infrastructure.SectionProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOS.PhysicalModel.Infrastructure;

public static class DependencyInjection
{
    public static void AddPhysicalModelInfrastructure(
        this ModelConfigurationBuilder configurationBuilder
    )
    {
        IEnumerable<Type> valueConverters = typeof(DependencyInjection)
            .Assembly
            .GetTypes()
            .Where(
                t =>
                    t.IsClass
                    && t.BaseType is not null
                    && t.BaseType.IsGenericType
                    && t.BaseType.GetGenericTypeDefinition() == typeof(ValueConverter<,>)
            );

        foreach (var valueConverterType in valueConverters)
        {
            var genericArgs = valueConverterType.BaseType?.GetGenericArguments();
            if (genericArgs?.Length != 2)
            {
                throw new ArgumentException();
            }

            _ = configurationBuilder.Properties(genericArgs[0]).HaveConversion(valueConverterType);
        }
    }

    public static IServiceCollection AddPhysicalModelInfrastructure(
        this IServiceCollection services
    )
    {
        _ = services.AddScoped<IRepository<ModelId, Model>, ModelDbContextRepository>();
        _ = services.AddScoped<IRepository<NodeId, Node>, NodeDbContextRepository>();
        _ = services.AddScoped<IRepository<Element1DId, Element1D>, Element1dDbContextRepository>();
        _ = services.AddScoped<IRepository<MaterialId, Material>, MaterialDbContextRepository>();
        _ = services.AddScoped<
            IRepository<SectionProfileId, SectionProfile>,
            SectionProfileDbContextRepository
        >();
        _ = services.AddScoped<IRepository<PointLoadId, PointLoad>, PointLoadDbContextRepository>();
        _ = services.AddScoped<
            IRepository<MomentLoadId, MomentLoad>,
            MomentLoadDbContextRepository
        >();

        return services;
    }
}
