using Microsoft.AspNetCore.Components;

namespace BeamOS.WebApp.Components.Providers;

public interface IRenderModeProvider
{
    IComponentRenderMode GlobalRenderMode { get; }
}
