using BeamOs.StructuralAnalysis.Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddStructuralAnalysisApplication(
        this IServiceCollection services
    )
    {
        return services.AddObjectThatImplementInterface<IAssemblyMarkerStructuralAnalysisApplication>(
            typeof(ICommandHandler<,>),
            ServiceLifetime.Scoped,
            false
        );
    }

    public static IServiceCollection AddObjectThatImplementInterface<TAssemblyMarker>(
        this IServiceCollection services,
        Type interfaceType,
        ServiceLifetime serviceLifetime,
        bool registerAsInterface
    )
    {
        IEnumerable<Type> assemblyTypes = typeof(TAssemblyMarker)
            .Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract);

        foreach (var assemblyType in assemblyTypes)
        {
            if (GetInterfaceType(assemblyType, interfaceType) is Type repoInterfaceType)
            {
                if (registerAsInterface)
                {
                    _ = serviceLifetime switch
                    {
                        ServiceLifetime.Transient
                            => services.AddTransient(repoInterfaceType, assemblyType),
                        ServiceLifetime.Scoped
                            => services.AddScoped(repoInterfaceType, assemblyType),
                        ServiceLifetime.Singleton
                            => services.AddSingleton(repoInterfaceType, assemblyType),
                        _ => throw new NotImplementedException(),
                    };
                }
                else
                {
                    _ = serviceLifetime switch
                    {
                        ServiceLifetime.Transient => services.AddTransient(assemblyType),
                        ServiceLifetime.Scoped => services.AddScoped(assemblyType),
                        ServiceLifetime.Singleton => services.AddSingleton(assemblyType),
                        _ => throw new NotImplementedException(),
                    };
                }
            }
        }

        return services;
    }

    public static IServiceCollection AddObjectThatExtendsBase<TAssemblyMarker>(
        this IServiceCollection services,
        Type baseType,
        ServiceLifetime serviceLifetime
    )
    {
        IEnumerable<Type> assemblyTypes = typeof(TAssemblyMarker)
            .Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract);

        foreach (var assemblyType in assemblyTypes)
        {
            if (ConcreteTypeDerivedFromBase(assemblyType, baseType))
            {
                _ = serviceLifetime switch
                {
                    ServiceLifetime.Transient => services.AddTransient(assemblyType),
                    ServiceLifetime.Scoped => services.AddScoped(assemblyType),
                    ServiceLifetime.Singleton => services.AddSingleton(assemblyType),
                    _ => throw new NotImplementedException(),
                };
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

    public static Type? GetConcreteBaseType(Type? concreteType, Type baseType)
    {
        bool isBaseTypeGeneric = baseType.IsGenericType;
        while (concreteType != null && concreteType != typeof(object))
        {
            if (
                isBaseTypeGeneric
                && concreteType.IsGenericType
                && concreteType.GetGenericTypeDefinition() == baseType
            )
            {
                return concreteType;
            }
            else if (!isBaseTypeGeneric && concreteType == baseType)
            {
                return concreteType;
            }
            concreteType = concreteType.BaseType;
        }

        return null;
    }

    public static bool ConcreteTypeDerivedFromBase(Type? concreteType, Type baseType)
    {
        bool isBaseTypeGeneric = baseType.IsGenericType;
        while (concreteType != null && concreteType != typeof(object))
        {
            if (
                isBaseTypeGeneric
                && concreteType.IsGenericType
                && concreteType.GetGenericTypeDefinition() == baseType
            )
            {
                return true;
            }
            else if (!isBaseTypeGeneric && concreteType == baseType)
            {
                return true;
            }
            concreteType = concreteType.BaseType;
        }

        return false;
    }
}
