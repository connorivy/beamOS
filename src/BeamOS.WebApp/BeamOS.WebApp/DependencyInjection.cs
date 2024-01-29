using BeamOS.WebApp.EditorApi;
using Microsoft.JSInterop;

namespace BeamOS.WebApp;

public static class DependencyInjection
{
    public static void RegisterSharedServices(this IServiceCollection services)
    {
        //_ = services.AddScoped(x => EditorApiProxy.Create(x.GetRequiredService<IJSRuntime>()));
        _ = services.AddTransient<EditorApiProxyFactory>();
    }
}
