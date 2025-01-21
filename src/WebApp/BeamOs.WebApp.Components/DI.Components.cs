using BeamOs.Common.Application;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.UndoRedo;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace BeamOs.WebApp.Components;

public static class DI
{
    public static IServiceCollection RegisterSharedServices<TAssembly>(
        this IServiceCollection services
    )
    {
        services.AddObjectThatExtendsBase<IAssemblyMarkerWebAppComponents>(
            typeof(CommandHandlerBase<>),
            ServiceLifetime.Scoped
        );
        services.AddFluxor(
            options =>
                options.ScanAssemblies(typeof(DI).Assembly).AddMiddleware<HistoryMiddleware>()
        );
        services.AddMudServices();
        services.AddScoped<UndoRedoFunctionality>();
        services.AddScoped<HistoryManager>();

        return services;
    }
}

public interface IAssemblyMarkerWebAppComponents { }
