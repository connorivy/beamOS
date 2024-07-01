using BeamOS.WebApp.Client.Features.KeyBindings.UndoRedo;
using BeamOS.WebApp.Client.Features.TestExplorer;
using BeamOS.WebApp.Client.State;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using MudBlazor.Services;
using MudExtensions.Services;

namespace BeamOS.WebApp.Client;

public static class DependencyInjection
{
    public static void RegisterSharedServices<TAssembly>(this IServiceCollection services)
    {
        //_ = services.AddScoped(x => EditorApiProxy.Create(x.GetRequiredService<IJSRuntime>()));
        _ = services.AddTransient<EditorApiProxyFactory>();
        _ = services.AddTransient<EditorEventsApi>();
        _ = services.AddMudServices().AddMudExtensions();

        _ = services.AddFluxor(o =>
        {
            o.ScanAssemblies(typeof(TAssembly).Assembly).AddMiddleware<UndoRedoMiddleware>();

            if (typeof(TAssembly) != typeof(DependencyInjection))
            {
                o.ScanAssemblies(typeof(DependencyInjection).Assembly);
            }
#if DEBUG
            o.UseReduxDevTools();
#endif
        });

        _ = services.AddSingleton<TestInfoProvider>();
        _ = services.AddSingleton<TestInfoStateProvider>();
        _ = services.AddScoped<HistoryManager>();
        _ = services.AddScoped<UndoRedoFunctionality>();
    }
}
