using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using BeamOs.Infrastructure.PhysicalModel.Element1Ds;
using BeamOs.Infrastructure.PhysicalModel.Materials;
using BeamOs.Infrastructure.PhysicalModel.Models;
using BeamOs.Infrastructure.PhysicalModel.MomentLoads;
using BeamOs.Infrastructure.PhysicalModel.Nodes;
using BeamOs.Infrastructure.PhysicalModel.PointLoads;
using BeamOs.Infrastructure.PhysicalModel.SectionProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Infrastructure.PhysicalModel;

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
