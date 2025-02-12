using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.AnalysisToolbar;

public sealed class RunDsmCommandCommandHandler(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher,
    ISnackbar snackbar
) : CommandHandlerBase<RunDsmCommand, AnalyticalResultsResponse>(snackbar)
{
    protected override async Task<Result<AnalyticalResultsResponse>> ExecuteCommandAsync(
        RunDsmCommand command,
        CancellationToken ct = default
    )
    {
        dispatcher.Dispatch(new AnalysisBegan() { ModelId = command.ModelId });

        var result = await structuralAnalysisApiClientV1.RunDirectStiffnessMethodAsync(
            command.ModelId,
            command.UnitsOverride,
            ct
        );

        if (result.IsSuccess)
        {
            dispatcher.Dispatch(
                new AnalyticalResultsCreated() { AnalyticalResults = result.Value }
            );
        }

        return result;
    }

    protected override void PostProcess(RunDsmCommand command)
    {
        dispatcher.Dispatch(new AnalysisEnded() { ModelId = command.ModelId });
    }
}
