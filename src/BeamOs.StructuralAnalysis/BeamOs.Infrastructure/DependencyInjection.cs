using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using BeamOs.Infrastructure.Repositories.PhysicalModel.Element1Ds;
using BeamOs.Infrastructure.Repositories.PhysicalModel.Materials;
using BeamOs.Infrastructure.Repositories.PhysicalModel.Models;
using BeamOs.Infrastructure.Repositories.PhysicalModel.MomentLoads;
using BeamOs.Infrastructure.Repositories.PhysicalModel.Nodes;
using BeamOs.Infrastructure.Repositories.PhysicalModel.PointLoads;
using BeamOs.Infrastructure.Repositories.PhysicalModel.SectionProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Infrastructure;

public static class DependencyInjection
{
    public static void AddPhysicalModelInfrastructure(
        this ModelConfigurationBuilder configurationBuilder
    )
    {
        var valueConverters = typeof(DependencyInjection)
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
        Type[] assemblyTypes = typeof(DependencyInjection)
            .Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .ToArray();

        foreach (var assemblyType in assemblyTypes)
        {
            if (GetGenericInterfaceType(assemblyType, typeof(IMapper<,>)) is not null)
            {
                _ = services.AddSingleton(assemblyType);
            }
            else if (
                GetGenericInterfaceType(assemblyType, typeof(IQueryHandler<,>))
                is Type interfaceType
            )
            {
                _ = services.AddScoped(interfaceType, assemblyType);
            }
            else if (
                GetGenericInterfaceType(assemblyType, typeof(IRepository<,>))
                is Type repoInterfaceType
            )
            {
                _ = services.AddScoped(repoInterfaceType, assemblyType);
            }
        }

        //_ = services.AddScoped<IRepository<ModelId, Model>, ModelDbContextRepository>();
        //_ = services.AddScoped<IRepository<NodeId, Node>, NodeDbContextRepository>();
        //_ = services.AddScoped<IRepository<Element1DId, Element1D>, Element1dDbContextRepository>();
        //_ = services.AddScoped<IRepository<MaterialId, Material>, MaterialDbContextRepository>();
        //_ = services.AddScoped<
        //    IRepository<SectionProfileId, SectionProfile>,
        //    SectionProfileDbContextRepository
        //>();
        //_ = services.AddScoped<IRepository<PointLoadId, PointLoad>, PointLoadDbContextRepository>();
        //_ = services.AddScoped<
        //    IRepository<MomentLoadId, MomentLoad>,
        //    MomentLoadDbContextRepository
        //>();

        return services;
    }

    private static bool TryGetGenericInterfaceType(
        Type concreteType,
        Type genericInterfaceType,
        out Type interfaceType
    )
    {
        foreach (var inter in concreteType.GetInterfaces())
        {
            if (inter.IsGenericType && inter.GetGenericTypeDefinition() == genericInterfaceType)
            {
                interfaceType = inter;
                return true;
            }
        }
        interfaceType = null;
        return false;
    }

    private static Type? GetGenericInterfaceType(Type concreteType, Type genericInterfaceType)
    {
        foreach (var inter in concreteType.GetInterfaces())
        {
            if (inter.IsGenericType && inter.GetGenericTypeDefinition() == genericInterfaceType)
            {
                return inter;
            }
        }
        return null;
    }

    public static IServiceCollection AddPhysicalModelInfrastructureReadModel(
        this IServiceCollection services,
        string connectionString
    )
    {
        return services.AddDbContext<BeamOsStructuralReadModelDbContext>(
            options => options.UseSqlServer(connectionString)
        );
    }
}
