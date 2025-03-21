using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.Nodes;

public sealed class PutNodeCommandHandler(
    IState<EditorComponentState> editorState,
    ISnackbar snackbar,
    PutNodeEditorCommandHandler putNodeEditorCommandHandler
) : CommandHandlerBase<PutNodeCommand, NodeResponse>(snackbar)
{
    protected override async Task<Result<NodeResponse>> ExecuteCommandAsync(
        PutNodeCommand command,
        CancellationToken ct = default
    )
    {
        var cachedModel = editorState.Value.CachedModelResponse;
        var currentNodeValue =
            cachedModel?.Nodes.GetValueOrDefault(command.Id)
            ?? throw new InvalidOperationException(
                "Cannot update node that doesn't exist in current model"
            );

        var newNodeValue = command.ToResponse();

        return await putNodeEditorCommandHandler.ExecuteAsync(
            new PutNodeClientCommand(currentNodeValue, newNodeValue),
            ct
        );
    }
}

public sealed class PutNodeEditorCommandHandler(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IState<EditorComponentState> editorState,
    IDispatcher dispatcher,
    ISnackbar snackbar
) : CommandHandlerBase<PutNodeClientCommand, NodeResponse>(snackbar)
{
    protected override async Task<Result<NodeResponse>> ExecuteCommandAsync(
        PutNodeClientCommand command,
        CancellationToken ct = default
    )
    {
        var request = command.New.ToRequest();
        return await structuralAnalysisApiClientV1.PutNodeAsync(
            command.New.Id,
            command.New.ModelId,
            request,
            ct
        );
    }

    protected override void PostProcess(PutNodeClientCommand command, Result<NodeResponse> response)
    {
        if (response.IsSuccess)
        {
            dispatcher.Dispatch(command with { HandledByServer = true, New = response.Value });
        }
    }
}
