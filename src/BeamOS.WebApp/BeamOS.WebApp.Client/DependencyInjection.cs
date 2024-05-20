using Microsoft.FluentUI.AspNetCore.Components;
using MudBlazor.Services;

namespace BeamOS.WebApp.Client;

public static class DependencyInjection
{
    public static void RegisterSharedServices(this IServiceCollection services)
    {
        //_ = services.AddScoped(x => EditorApiProxy.Create(x.GetRequiredService<IJSRuntime>()));
        _ = services.AddTransient<EditorApiProxyFactory>();
        _ = services.AddFluentUIComponents();
        _ = services.AddMudServices();
    }
}
