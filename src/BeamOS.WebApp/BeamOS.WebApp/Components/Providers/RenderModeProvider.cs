using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BeamOS.WebApp.Components.Providers;

public class RenderModeProvider : IRenderModeProvider
{
    public IComponentRenderMode GlobalRenderMode { get; } = new InteractiveServerRenderMode(false);
}
