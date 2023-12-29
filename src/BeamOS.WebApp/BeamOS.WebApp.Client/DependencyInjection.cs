using BeamOS.WebApp.EditorApi;
using Microsoft.JSInterop;

namespace BeamOS.WebApp.Client;

public static class DependencyInjection
{
    public static void RegisterSharedServices(this IServiceCollection services)
    {
        _ = services.AddScoped<IEditorApiAlpha>(
            x => EditorApiProxy.Create(x.GetRequiredService<IJSRuntime>())
        );
    }
}
