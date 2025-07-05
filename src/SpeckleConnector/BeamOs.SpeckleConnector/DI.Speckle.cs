using System;
using BeamOs.Common.Api;
using BeamOs.Common.Application;
using Microsoft.Extensions.DependencyInjection;
using Speckle.Sdk;

namespace BeamOs.SpeckleConnector;

public static class DI
{
    public static IServiceCollection AddSpeckleRequired(this IServiceCollection services)
    {
        // services.AddSingleton<BeamOsSpeckleReceiveOperation>();
        services.AddScoped<BeamOsSpeckleReceiveOperation2>();
        services.AddScoped<SpeckleReceiveOperationContext>();
        services.AddScoped<SpeckleRecieveOperation>();

        services.AddObjectThatExtendsBase<IAssemblyMarkerSpeckleConnector>(
            typeof(ToProposalConverter<>),
            ServiceLifetime.Scoped
        );

        services.AddObjectThatExtendsBase<IAssemblyMarkerSpeckleConnector>(
            typeof(BeamOsBaseEndpoint<,>),
            ServiceLifetime.Scoped
        );

        Speckle.Sdk.Application application = new("local", "local");
        services.AddSpeckleSdk(
            application,
            "1.0.0",
            typeof(Speckle.Objects.Data.RevitObject).Assembly
        );

        return services;
    }
}
