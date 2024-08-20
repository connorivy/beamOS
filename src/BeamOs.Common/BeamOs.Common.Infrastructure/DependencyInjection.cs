using BeamOs.Common.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Common.Infrastructure;

public static class DependencyInjection
{
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

    public static IServiceCollection AddMappers<TAssemblyMarker>(this IServiceCollection services)
    {
        IEnumerable<Type> assemblyTypes = typeof(TAssemblyMarker)
            .Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract);

        foreach (var assemblyType in assemblyTypes)
        {
            if (GetInterfaceType(assemblyType, typeof(IMapper<,>)) is not null)
            {
                _ = services.AddSingleton(assemblyType);
            }
        }

        return services;
    }

    public static IServiceCollection AddQueryHandlers<TAssemblyMarker>(
        this IServiceCollection services
    )
    {
        IEnumerable<Type> assemblyTypes = typeof(TAssemblyMarker)
            .Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract);

        foreach (var assemblyType in assemblyTypes)
        {
            if (GetInterfaceType(assemblyType, typeof(IQueryHandler<,>)) is Type interfaceType)
            {
                _ = services.AddScoped(interfaceType, assemblyType);
            }
        }

        return services;
    }

    public static IServiceCollection AddRepositories<TAssemblyMarker>(
        this IServiceCollection services
    )
    {
        IEnumerable<Type> assemblyTypes = typeof(TAssemblyMarker)
            .Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract);

        foreach (var assemblyType in assemblyTypes)
        {
            if (GetInterfaceType(assemblyType, typeof(IRepository<,>)) is Type repoInterfaceType)
            {
                _ = services.AddScoped(repoInterfaceType, assemblyType);
            }
        }

        return services;
    }

    public static Type? GetInterfaceType(Type concreteType, Type interfaceType)
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
}
