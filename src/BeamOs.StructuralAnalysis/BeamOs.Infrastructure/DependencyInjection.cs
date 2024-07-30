using BeamOs.Application.AnalyticalResults.Diagrams.ShearDiagrams.Interfaces;
using BeamOs.Application.AnalyticalResults.ModelResults;
using BeamOs.Application.AnalyticalResults.NodeResults;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Application.PhysicalModel.Element1dAggregate;
using BeamOs.Application.PhysicalModel.Materials;
using BeamOs.Application.PhysicalModel.Models;
using BeamOs.Application.PhysicalModel.MomentLoads;
using BeamOs.Application.PhysicalModel.Nodes.Interfaces;
using BeamOs.Application.PhysicalModel.PointLoads;
using BeamOs.Application.PhysicalModel.SectionProfiles;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Infrastructure.Interceptors;
using BeamOs.Infrastructure.Repositories.AnalyticalResults.Diagrams.ShearDiagrams;
using BeamOs.Infrastructure.Repositories.AnalyticalResults.ModelResults;
using BeamOs.Infrastructure.Repositories.AnalyticalResults.NodeResults;
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
            if (GetInterfaceType(assemblyType, typeof(IMapper<,>)) is not null)
            {
                _ = services.AddSingleton(assemblyType);
            }
            else if (GetInterfaceType(assemblyType, typeof(IQueryHandler<,>)) is Type interfaceType)
            {
                _ = services.AddScoped(interfaceType, assemblyType);
            }
            else if (
                GetInterfaceType(assemblyType, typeof(IRepository<,>)) is Type repoInterfaceType
            )
            {
                _ = services.AddScoped(repoInterfaceType, assemblyType);
            }
        }

        _ = services
            .AddScoped<IElement1dRepository, Element1dDbContextRepository>()
            .AddScoped<IMaterialRepository, MaterialDbContextRepository>()
            .AddScoped<IModelRepository, ModelDbContextRepository>()
            .AddScoped<IMomentLoadRepository, MomentLoadDbContextRepository>()
            .AddScoped<INodeRepository, NodeDbContextRepository>()
            .AddScoped<IPointLoadRepository, PointLoadDbContextRepository>()
            .AddScoped<ISectionProfileRepository, SectionProfileDbContextRepository>()
            .AddScoped<IModelResultRepository, ModelResultDbContextRepository>()
            .AddScoped<INodeResultRepository, NodeResultDbContextRepository>()
            .AddScoped<IShearDiagramRepository, ShearForceDiagramDbContextRepository>()
            .AddScoped<IMomentDiagramRepository, MomentDiagramDbContextRepository>();

        _ = services.AddScoped<IUnitOfWork, UnitOfWork>();
        _ = services.AddScoped<PublishIntegrationEventsInterceptor>();

        return services;
    }

    private static Type? GetInterfaceType(Type concreteType, Type interfaceType)
    {
        bool isGeneric = interfaceType.IsGenericType;
        foreach (var inter in concreteType.GetInterfaces())
        {
            if (
                isGeneric
                && inter.IsGenericType
                && inter.GetGenericTypeDefinition() == interfaceType
            )
            {
                return inter;
            }
            else if (!isGeneric && inter == interfaceType)
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
