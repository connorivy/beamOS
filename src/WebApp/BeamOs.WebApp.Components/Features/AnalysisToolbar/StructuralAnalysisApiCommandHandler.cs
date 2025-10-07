using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.AnalysisToolbar;

public sealed class RunDsmCommandCommandHandler(
    BeamOsResultApiClient apiClient,
    IDispatcher dispatcher,
    ISnackbar snackbar,
    ILogger<RunDsmCommandCommandHandler> logger
)
    : CommandHandlerBase<ModelResourceRequest<RunDsmRequest>, AnalyticalResultsResponse>(
        snackbar,
        logger
    )
{
    protected override async Task<Result<AnalyticalResultsResponse>> ExecuteCommandAsync(
        ModelResourceRequest<RunDsmRequest> command,
        CancellationToken ct = default
    )
    {
        dispatcher.Dispatch(new AnalysisBegan() { ModelId = command.ModelId });

        var result = await apiClient
            .Models[command.ModelId]
            .Analyze.Dsm.RunDirectStiffnessMethodAsync(
                new()
                {
                    LoadCombinationIds = command.Body.LoadCombinationIds,
                    UnitsOverride = command.Body.UnitsOverride,
                },
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

    protected override void PostProcess(
        ModelResourceRequest<RunDsmRequest> command,
        Result<AnalyticalResultsResponse> _
    )
    {
        dispatcher.Dispatch(new AnalysisEnded() { ModelId = command.ModelId });
    }
}

public sealed class RunOpenSeesCommandCommandHandler(
    BeamOsResultApiClient apiClient,
    IDispatcher dispatcher,
    ISnackbar snackbar,
    ILogger<RunOpenSeesCommandCommandHandler> logger
)
    : CommandHandlerBase<ModelResourceRequest<RunDsmRequest>, AnalyticalResultsResponse>(
        snackbar,
        logger
    )
{
    protected override async Task<Result<AnalyticalResultsResponse>> ExecuteCommandAsync(
        ModelResourceRequest<RunDsmRequest> command,
        CancellationToken ct = default
    )
    {
        dispatcher.Dispatch(new AnalysisBegan() { ModelId = command.ModelId });

        var result = await apiClient
            .Models[command.ModelId]
            .Analyze.Opensees.RunOpenSeesAnalysisAsync(
                new()
                {
                    LoadCombinationIds = command.Body.LoadCombinationIds,
                    UnitsOverride = command.Body.UnitsOverride,
                },
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

    protected override void PostProcess(
        ModelResourceRequest<RunDsmRequest> command,
        Result<AnalyticalResultsResponse> _
    )
    {
        dispatcher.Dispatch(new AnalysisEnded() { ModelId = command.ModelId });
    }
}
