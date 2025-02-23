using Microsoft.JSInterop;

namespace BeamOs.WebApp.Components.Features.ResultCharts;

public partial class ResultChartsComponent(IJSRuntime jSRuntime)
{
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await jSRuntime.InvokeVoidAsync("createCharts", "hi");
        }
    }
}
