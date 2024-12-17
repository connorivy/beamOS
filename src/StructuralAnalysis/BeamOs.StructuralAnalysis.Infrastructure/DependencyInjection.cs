using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddStructuralAnalysisInfrastructure(
        this IServiceCollection services,
        string connectionString
    )
    {
        _ = services.AddScoped<INodeRepository, NodeRepository>();
        _ = services.AddDbContext<StructuralAnalysisDbContext>(options =>
            options.UseNpgsql(connectionString).UseModel(StructuralAnalysisDbContextModel.Instance)
        );

        _ = services.AddScoped<IStructuralAnalysisUnitOfWork, UnitOfWork>();

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
            .Assembly.GetTypes()
            .Where(t =>
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
