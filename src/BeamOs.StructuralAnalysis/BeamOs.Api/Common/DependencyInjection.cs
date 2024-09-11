using BeamOs.Common.Api;
using BeamOs.Common.Application.Interfaces;

namespace BeamOs.Api.Common;

public static class DependencyInjection
{
    public static IServiceCollection AddBeamOsEndpoints<T>(this IServiceCollection services)
    {
        _ = services.Scan(
            scan =>
                scan.FromAssemblyOf<T>()
                    //.AddClasses(classes => classes.AssignableTo(typeof(BeamOsEndpoint<,>)))
                    //.AddClasses(classes => classes.AssignableTo(typeof(BeamOsEndpoint<,,>)))
                    //.AddClasses(classes => classes.AssignableTo(typeof(BeamOsEndpoint<,,,>)))
                    //.AddClasses(classes => classes.AssignableTo(typeof(BeamOsEndpoint<,,,>)))
                    .AddClasses(classes => classes.AssignableTo(typeof(IBeamOsEndpoint<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
        );

        return services;
    }

    //public static void AddBeamOsEndpoints<T>(this IEndpointRouteBuilder app)
    //{
    //    IEnumerable<Type> endpointTypes = typeof(T)
    //        .Assembly
    //        .GetTypes()
    //        .Where(
    //            t => !t.IsAbstract && !t.IsInterface && t.IsAssignableTo(typeof(IBeamOsEndpointBase))
    //        );

    //    var scope = app.ServiceProvider.CreateScope();
    //    foreach (var type in endpointTypes)
    //    {
    //        var endpoint = scope.ServiceProvider.GetService(type) as BeamOsEndpointBase;
    //        endpoint?.Map(app);
    //    }
    //}

    public static IServiceCollection AddMappers<T>(this IServiceCollection services)
    {
        _ = services.Scan(
            scan =>
                scan.FromAssemblyOf<T>()
                    .AddClasses(classes => classes.AssignableTo(typeof(IMapper<,>)))
                    .AsSelf()
                    //.AsImplementedInterfaces()
                    .WithTransientLifetime()
        );

        return services;
    }
}
