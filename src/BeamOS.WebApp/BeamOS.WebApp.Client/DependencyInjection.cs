using MudBlazor.Services;
using MudExtensions.Services;

namespace BeamOS.WebApp.Client;

public static class DependencyInjection
{
    public static void RegisterSharedServices(this IServiceCollection services)
    {
        //_ = services.AddScoped(x => EditorApiProxy.Create(x.GetRequiredService<IJSRuntime>()));
        _ = services.AddTransient<EditorApiProxyFactory>();
        _ = services.AddMudServices().AddMudExtensions();
    }
}
