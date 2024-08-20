using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Application.Common;

public static class DependencyInjection
{
    public static IServiceCollection AddCommandHandlers<T>(this IServiceCollection services)
    {
        _ = services.Scan(
            scan =>
                scan.FromAssemblyOf<T>()
                    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
                    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandlerSync<,>)))
                    .AsSelf()
                    //.AsImplementedInterfaces()
                    .WithTransientLifetime()
        );

        return services;
    }

    public static IServiceCollection AddServicesAsType<T>(
        this IServiceCollection services,
        Type serviceType,
        ServiceLifetime serviceLifetime
    )
    {
        Type[] assemblyTypes = typeof(T)
            .Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .ToArray();

        Func<Type, Type, Type?> serviceDiscoverer;
        if (serviceType.IsInterface)
        {
            serviceDiscoverer = GetInterfaceType;
        }
        else
        {
            serviceDiscoverer = GetConcreteType;
        }

        foreach (var assemblyType in assemblyTypes)
        {
            if (serviceDiscoverer(assemblyType, serviceType) is Type serviceToRegister)
            {
                _ = serviceLifetime switch
                {
                    ServiceLifetime.Transient
                        => services.AddTransient(serviceToRegister, assemblyType),
                    ServiceLifetime.Scoped => services.AddScoped(serviceToRegister, assemblyType),
                    ServiceLifetime.Singleton
                        => services.AddSingleton(serviceToRegister, assemblyType),
                    ServiceLifetime.Undefined
                    or _
                        => throw new Exception("Invalid value for service lifetime"),
                };
            }
        }

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

    private static Type? GetConcreteType(Type concreteType, Type baseType)
    {
        if (concreteType == baseType)
        {
            return baseType;
        }

        if (concreteType.BaseType == typeof(object) || concreteType.BaseType is null)
        {
            return null;
        }

        bool isGeneric = baseType.IsGenericType;

        if (
            isGeneric
            && concreteType.BaseType.IsGenericType
            && concreteType.BaseType.GetGenericTypeDefinition() == baseType
        )
        {
            return baseType;
        }
        else if (!isGeneric && concreteType.BaseType == baseType)
        {
            return baseType;
        }
        return GetConcreteType(concreteType.BaseType, baseType);
    }
}

public enum ServiceLifetime
{
    Undefined = 0,
    Transient = 1,
    Scoped = 2,
    Singleton = 3
}
