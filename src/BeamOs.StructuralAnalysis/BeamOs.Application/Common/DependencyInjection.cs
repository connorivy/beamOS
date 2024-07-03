using BeamOs.Application.Common.Interfaces;
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
}
