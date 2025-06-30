using System.Collections.Immutable;
using System.Diagnostics;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.WebApp.Components.Features.ResultCharts;

public partial class ResultChartsComponent(
    IJSRuntime jSRuntime,
    IStateSelection<CachedModelState, CachedModelResponse> cachedModelsState,
    IState<EditorComponentState> editorComponentState,
    IDispatcher dispatcher,
    IState<ResultChartsState> state
) : FluxorComponent
{
    [Parameter]
    public Guid ModelId { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        cachedModelsState.Select(m => m.Models[this.ModelId]);

        this.SubscribeToAction<ChangeSelectionCommand>(action =>
        {
            if (
                action.SelectedObjects.Length == 0
                || action.SelectedObjects[0].ObjectType != BeamOsObjectType.Element1d
            )
            {
                dispatcher.Dispatch(new ResultsChanged());
                return;
            }
            int element1dId = editorComponentState.Value.SelectedObjects[0].Id;
            if (
                cachedModelsState.Value.DeflectionDiagrams is null
                || !cachedModelsState.Value.DeflectionDiagrams.TryGetValue(
                    element1dId,
                    out var deflectionDiagram
                )
            )
            {
                dispatcher.Dispatch(new ResultsChanged());
                return;
            }

            this.GetSelectedMemberChartValues(element1dId, deflectionDiagram);
        });
    }

    private void GetSelectedMemberChartValues(
        int element1dId,
        DeflectionDiagramResponse deflectionDiagram
    )
    {
        //if (
        //    editorComponentState.Value.SelectedObjects.Length == 0
        //    || editorComponentState.Value.SelectedObjects[0].TypeName != "Element1d"
        //)
        //{
        //    this.chartXValues = null;
        //    this.deflectionValues = null;
        //    return;
        //}
        //int element1dId = editorComponentState.Value.SelectedObjects[0].Id;
        //if (
        //    cachedModelsState.Value.DeflectionDiagrams is null
        //    || !cachedModelsState
        //        .Value
        //        .DeflectionDiagrams
        //        .TryGetValue(element1dId, out var deflectionDiagram)
        //)
        //{
        //    return;
        //}

        var shearDiagram = cachedModelsState.Value.ShearDiagrams[element1dId];
        var momentDiagram = cachedModelsState.Value.MomentDiagrams[element1dId];

        double element1dLength = shearDiagram.Intervals.Last().EndLocation.Value;

        var regularIntervalLocations = Enumerable
            .Range(0, deflectionDiagram.NumSteps)
            .Select(i => i * (element1dLength / (deflectionDiagram.NumSteps - 1)))
            .ToImmutableSortedSet();
        List<double> relativeOffsets = new(regularIntervalLocations.Count);
        for (int i = 0; i < regularIntervalLocations.Count; i++)
        {
            relativeOffsets.Add(deflectionDiagram.Offsets[i * 3 + 1]);
        }

        int index = 0;
        var evalPoints = shearDiagram
            .Intervals.SelectMany(i => new double[] { i.StartLocation.Value, i.EndLocation.Value })
            .Concat(
                momentDiagram.Intervals.SelectMany(i =>
                    new double[] { i.StartLocation.Value, i.EndLocation.Value }
                )
            )
            .Concat(regularIntervalLocations)
            .Order()
            .Distinct()
            .ToList();
        //.Select(i => (i, index++))
        //.OrderBy(kvp => kvp.i);

        List<double> shearValues = new(evalPoints.Count);
        List<double> momentValues = new(evalPoints.Count);

        int numDiagramXValues = 0;
        int numDuplicateXValues = 0;
        int originalEvalPointCount = evalPoints.Count;
        for (int i = 0; i < originalEvalPointCount; i++)
        {
            int iEff = i + numDuplicateXValues;
            double location = evalPoints[iEff];

            if (!regularIntervalLocations.Any(value => Math.Abs(value - location) <= 1e-4))
            {
                Debug.Assert(i != evalPoints.Count - 1);

                double prevX = regularIntervalLocations[i - numDiagramXValues];
                double prevValue = relativeOffsets[iEff];
                double nextX = regularIntervalLocations[i - numDiagramXValues + 1];
                double nextValue = relativeOffsets[iEff + 1];

                double interpolated =
                    prevX + (location - prevX) / (nextX - prevX) * (nextValue - prevValue);
                relativeOffsets.Insert(iEff, interpolated);

                numDiagramXValues++;
            }
            var (shearValOnLeft, shearValOnRight) = shearDiagram.Intervals.GetValueAtLocation(
                new Length(location, LengthUnit.Meter),
                new Length(1, LengthUnit.Inch),
                out bool isBetweenIntervals
            );
            var (momValOnLeft, momValOnRight) = momentDiagram.Intervals.GetValueAtLocation(
                new Length(location, LengthUnit.Meter),
                new Length(1, LengthUnit.Inch),
                out bool isBetweenMomIntervals
            );

            shearValues.Add(shearValOnLeft);
            momentValues.Add(momValOnLeft);

            if (
                (isBetweenIntervals && Math.Abs(shearValOnLeft - shearValOnRight) > .001)
                || (isBetweenMomIntervals && Math.Abs(momValOnLeft - momValOnRight) > .001)
            )
            {
                // there is a jump in one of the graphs, we need to add a point
                evalPoints.Insert(iEff, location);
                if (iEff == relativeOffsets.Count)
                {
                    relativeOffsets.Add(relativeOffsets.Last());
                }
                else
                {
                    relativeOffsets.Insert(iEff + 1, relativeOffsets[iEff]);
                }
                shearValues.Add(shearValOnRight);
                momentValues.Add(momValOnRight);
                numDuplicateXValues++;
            }
        }

        dispatcher.Dispatch(
            new ResultsChanged()
            {
                ChartXValues = evalPoints,
                DeflectionValues = relativeOffsets,
                ShearValues = shearValues,
                MomentValues = momentValues,
            }
        );

        return;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await jSRuntime.InvokeVoidAsync("resultCharts.init");
        }

        //this.GetSelectedMemberChartValues();

        if (state.Value.ChartXValues is not null)
        {
            await jSRuntime.InvokeVoidAsync(
                "resultCharts.createCharts",
                state.Value.ChartXValues,
                state.Value.ShearValues,
                state.Value.MomentValues,
                state.Value.DeflectionValues
            );
        }
        else
        {
            await jSRuntime.InvokeVoidAsync("resultCharts.destroyCharts");
        }
    }
}

[FeatureState]
public record ResultChartsState(
    List<double>? ChartXValues,
    List<double>? DeflectionValues,
    List<double>? ShearValues,
    List<double>? MomentValues
)
{
    public ResultChartsState()
        : this(null, null, null, null) { }
}

public readonly record struct ResultsChanged(
    List<double>? ChartXValues,
    List<double>? DeflectionValues,
    List<double>? ShearValues,
    List<double>? MomentValues
);

public static class ResultChartsStateReducers
{
    [ReducerMethod]
    public static ResultChartsState Reduce(ResultChartsState state, ResultsChanged action) =>
        state with
        {
            ChartXValues = action.ChartXValues,
            DeflectionValues = action.DeflectionValues,
            ShearValues = action.ShearValues,
            MomentValues = action.MomentValues,
        };
}
