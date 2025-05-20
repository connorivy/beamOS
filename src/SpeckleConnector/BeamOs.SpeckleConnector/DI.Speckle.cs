using System;
using BeamOs.Common.Api;
using BeamOs.Common.Application;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.SpeckleConnector;

public static class DI
{
    public static IServiceCollection AddSpeckleRequired(this IServiceCollection services)
    {
        // services.AddSingleton<BeamOsSpeckleReceiveOperation>();
        services.AddScoped<BeamOsSpeckleReceiveOperation2>();
        services.AddScoped<SpeckleReceiveOperationContext>();
        services.AddScoped<SpeckleRecieveOperation>();

        services.AddObjectThatImplementInterface<IAssemblyMarkerSpeckleConnector>(
            typeof(ITopLevelProposalConverter<>),
            ServiceLifetime.Scoped,
            true
        );

        services.AddObjectThatExtendsBase<IAssemblyMarkerSpeckleConnector>(
            typeof(ToProposalConverter<>),
            ServiceLifetime.Scoped
        );

        services.AddObjectThatExtendsBase<IAssemblyMarkerSpeckleConnector>(
            typeof(BeamOsBaseEndpoint<,>),
            ServiceLifetime.Scoped
        );

        return services;
    }
}
