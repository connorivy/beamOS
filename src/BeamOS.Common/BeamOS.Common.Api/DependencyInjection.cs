using BeamOS.Common.Api.Interfaces;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOS.Common.Api;
public static class DependencyInjection
{
    public static IServiceCollection AddBeamOsEndpoints<T>(this IServiceCollection services)
    {
        _ = services.Scan(scan => scan
            .FromAssemblyOf<T>()
            .AddClasses(classes => classes.AssignableTo(typeof(BaseEndpoint)))
            .AsSelf()
            //.AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        return services;
    }

    public static void AddBeamOsEndpoints<T>(this IEndpointRouteBuilder app)
    {
        IEnumerable<Type> endpointTypes = typeof(T).Assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && t.IsAssignableTo(typeof(BaseEndpoint)));

        var scope = app.ServiceProvider.CreateScope();
        foreach (Type type in endpointTypes)
        {
            var endpoint = scope.ServiceProvider.GetService(type) as BaseEndpoint;
            endpoint?.Map(app);
        }
    }

    public static IServiceCollection AddMappers<T>(this IServiceCollection services)
    {
        _ = services.Scan(scan => scan
            .FromAssemblyOf<T>()
            .AddClasses(classes => classes.AssignableTo(typeof(IMapper<,>)))
            .AsSelf()
            //.AsImplementedInterfaces()
            .WithTransientLifetime()
        );

        return services;
    }
}
