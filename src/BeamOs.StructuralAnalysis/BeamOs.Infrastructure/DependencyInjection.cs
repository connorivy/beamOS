using BeamOs.Application.Common.Interfaces;
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

        return services;
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
