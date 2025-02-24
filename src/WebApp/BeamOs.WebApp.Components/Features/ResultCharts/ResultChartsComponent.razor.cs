using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BeamOs.WebApp.Components.Features.ResultCharts;

public partial class ResultChartsComponent(
    IJSRuntime jSRuntime,
    IStateSelection<CachedModelState, CachedModelResponse> cachedModelsState,
    IState<EditorComponentState> editorComponentState
) : FluxorComponent
{
    [Parameter]
    public Guid ModelId { get; set; }

    private double[]? chartXValues;
    private double[]? chartYValues;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        cachedModelsState.Select(m => m.Models[this.ModelId]);
    }

    private void GetSelectedMemberChartValues()
    {
        if (
            editorComponentState.Value.SelectedObjects.Length == 0
            || editorComponentState.Value.SelectedObjects[0].TypeName != "Element1d"
        )
        {
            this.chartXValues = null;
            this.chartYValues = null;
            return;
        }
        int element1dId = editorComponentState.Value.SelectedObjects[0].Id;
        if (
            cachedModelsState.Value.DeflectionDiagrams is null
            || !cachedModelsState
                .Value
                .DeflectionDiagrams
                .TryGetValue(element1dId, out var deflectionDiagram)
        )
        {
            return;
        }

        double[] relativeOffsets = new double[deflectionDiagram.Offsets.Length / 3];
        for (int i = 0; i < deflectionDiagram.NumSteps; i++)
        {
            relativeOffsets[i] = Math.Sqrt(
                Math.Pow(deflectionDiagram.Offsets[i * 3], 2)
                    + Math.Pow(deflectionDiagram.Offsets[i * 3 + 1], 2)
                    + Math.Pow(deflectionDiagram.Offsets[i * 3 + 2], 2)
            );
        }

        this.chartYValues = relativeOffsets;
        this.chartXValues =
        [
            .. Enumerable.Range(0, deflectionDiagram.Offsets.Length / 3).Select(i => i * .1)
        ];
        return;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        this.GetSelectedMemberChartValues();

        if (this.chartXValues is not null)
        {
            await jSRuntime.InvokeVoidAsync(
                "resultCharts.createCharts",
                this.chartXValues,
                this.chartXValues,
                this.chartXValues,
                this.chartYValues
            );
        }
        else
        {
            await jSRuntime.InvokeVoidAsync(
                "resultCharts.destroyCharts",
                this.chartXValues,
                this.chartXValues,
                this.chartXValues,
                this.chartYValues
            );
        }
    }
}
