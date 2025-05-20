using BeamOs.CodeGen.SpeckleConnectorApi;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.AnalysisToolbar;

public sealed class ReceiveFromSpeckleCommandHandler(
    ISpeckleConnectorApi speckleConnectorApi,
    IDispatcher dispatcher,
    ISnackbar snackbar,
    ILogger<ReceiveFromSpeckleCommandHandler> logger
) : CommandHandlerBase<ReceiveFromSpeckleCommand, ModelProposalResponse>(snackbar, logger)
{
    protected override async Task<Result<ModelProposalResponse>> ExecuteCommandAsync(
        ReceiveFromSpeckleCommand command,
        CancellationToken ct = default
    )
    {
        dispatcher.Dispatch(
            new EditorLoadingBegin(command.CanvasId, "Receiving Data From Spackle")
        );

        return await speckleConnectorApi.SpeckleRecieveOperationAsync(
            command.ModelId,
            command.SpeckleReceiveParameters,
            ct
        );
    }

    protected override void PostProcess(
        ReceiveFromSpeckleCommand command,
        Result<ModelProposalResponse> result
    )
    {
        dispatcher.Dispatch(new EditorLoadingEnd() { CanvasId = command.CanvasId });
    }
}

public readonly record struct ReceiveFromSpeckleCommand(
    string CanvasId,
    Guid ModelId,
    SpeckleReceiveParameters SpeckleReceiveParameters
);
