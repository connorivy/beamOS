using System.Collections.Generic;
using BeamOs.Common.Application;
using BeamOs.StructuralAnalysis.Application;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.NodeResults;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.NodeResults;
using BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Infrastructure.Common;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.SectionProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddStructuralAnalysisInfrastructureRequired(
        this IServiceCollection services
    )
    {
        _ = services.AddScoped<INodeRepository, NodeRepository>();
        _ = services.AddScoped<IModelRepository, ModelRepository>();
        _ = services.AddScoped<IMaterialRepository, MaterialRepository>();
        _ = services.AddScoped<ISectionProfileRepository, SectionProfileRepository>();
        _ = services.AddScoped<IElement1dRepository, Element1dRepository>();
        _ = services.AddScoped<IPointLoadRepository, PointLoadRepository>();
        _ = services.AddScoped<IMomentLoadRepository, MomentLoadRepository>();
        _ = services.AddScoped<INodeResultRepository, NodeResultRepository>();
        _ = services.AddScoped<IResultSetRepository, ResultSetRepository>();

        _ = services.AddScoped<IStructuralAnalysisUnitOfWork, UnitOfWork>();

        services.AddObjectThatImplementInterface<IAssemblyMarkerInfrastructure>(
            typeof(IQueryHandler<,>),
            ServiceLifetime.Scoped,
            false
        );

        services.AddScoped<
            IQueryHandler<EmptyRequest, List<ModelInfoResponse>>,
            GetModelsQueryHandler
        >();

        return services;
    }

    public static IServiceCollection AddStructuralAnalysisInfrastructureConfigurable(
        this IServiceCollection services,
        string connectionString
    )
    {
        _ = services.AddDbContext<StructuralAnalysisDbContext>(options =>
        {
            options
                .UseSqlServer(connectionString)
                .AddInterceptors(new ModelEntityIdIncrementingInterceptor());

            //var optionsBuilderNoInterceptor =
            //    options.UseSqlServer(connectionString).Options
            //    as DbContextOptions<StructuralAnalysisDbContext>;

            //options.AddInterceptors(new IdentityInsertInterceptor(optionsBuilderNoInterceptor));
        }
        //.UseModel(StructuralAnalysisDbContextModel.Instance)
        );

        return services;
    }

    public static void AddPhysicalModelInfrastructure(
        this ModelConfigurationBuilder configurationBuilder
    )
    {
        configurationBuilder.AddValueConverters<IAssemblyMarkerInfrastructure>();
    }

    public static void AddValueConverters<TAssemblyMarker>(
        this ModelConfigurationBuilder configurationBuilder
    )
    {
        var valueConverters = typeof(TAssemblyMarker)
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
}
