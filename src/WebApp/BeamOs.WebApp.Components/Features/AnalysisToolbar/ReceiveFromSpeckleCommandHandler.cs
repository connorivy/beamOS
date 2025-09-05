using System.Collections.Immutable;
using BeamOs.CodeGen.SpeckleConnectorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.ProposalInfo;
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
        if (result.IsSuccess && result.Value.ModelProposal is not null)
        {
            dispatcher.Dispatch(
                new ProposalInfoState.ModelProposalInfoLoaded(result.Value.ModelProposal)
            );
        }
    }
}

public readonly record struct ReceiveFromSpeckleCommand(
    string CanvasId,
    Guid ModelId,
    SpeckleReceiveParameters SpeckleReceiveParameters
);

public readonly record struct ModelRepairRequest(string CanvasId, Guid ModelId);

public sealed class ModelRepairClientCommandHandler(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClient,
    IDispatcher dispatcher,
    ISnackbar snackbar,
    ILogger<ModelRepairClientCommandHandler> logger
) : CommandHandlerBase<ModelRepairRequest, ModelProposalResponse>(snackbar, logger)
{
    protected override async Task<Result<ModelProposalResponse>> ExecuteCommandAsync(
        ModelRepairRequest command,
        CancellationToken ct = default
    )
    {
        dispatcher.Dispatch(new EditorLoadingBegin(command.CanvasId, "Repairing Model"));

        return await structuralAnalysisApiClient.RepairModelAsync(command.ModelId, "", ct);
    }

    protected override void PostProcess(
        ModelRepairRequest command,
        Result<ModelProposalResponse> result
    )
    {
        dispatcher.Dispatch(new EditorLoadingEnd() { CanvasId = command.CanvasId });
        if (result.IsSuccess && result.Value.ModelProposal is not null)
        {
            dispatcher.Dispatch(
                new ProposalInfoState.ModelProposalInfoLoaded(result.Value.ModelProposal)
            );
        }
    }
}

public sealed class GetModelProposalsClientCommandHandler(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClient,
    IDispatcher dispatcher,
    ISnackbar snackbar,
    ILogger<GetModelProposalsClientCommandHandler> logger
) : CommandHandlerBase<ModelRepairRequest, ICollection<ModelProposalInfo>>(snackbar, logger)
{
    protected override async Task<Result<ICollection<ModelProposalInfo>>> ExecuteCommandAsync(
        ModelRepairRequest command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClient.GetModelProposalsAsync(command.ModelId, ct);
    }

    protected override void PostProcess(
        ModelRepairRequest command,
        Result<ICollection<ModelProposalInfo>> result
    )
    {
        if (result.IsSuccess && result.Value is not null)
        {
            dispatcher.Dispatch(new ProposalInfoState.ModelProposalInfosLoaded([.. result.Value]));
        }
    }
}
