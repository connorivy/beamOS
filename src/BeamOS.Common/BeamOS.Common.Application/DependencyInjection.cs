using BeamOS.Common.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOS.Common.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddCommandHandlers<T>(this IServiceCollection services)
    {
        _ = services.Scan(scan => scan
           .FromAssemblyOf<T>()
           .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
           .AsSelf()
           //.AsImplementedInterfaces()
           .WithTransientLifetime()
       );

        return services;
    }
}
