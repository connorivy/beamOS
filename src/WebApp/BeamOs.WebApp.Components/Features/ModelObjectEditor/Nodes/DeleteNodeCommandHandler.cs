using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.ModelObjectEditor.MomentLoads;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.Nodes;

public sealed class DeleteNodeCommandHandler(
    ILogger<DeleteNodeCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState
) : ClientCommandHandlerBase<DeleteNodeClientCommand, ModelEntityResponse>(logger, snackbar)
{
    protected override async ValueTask<Result> UpdateEditor(DeleteNodeClientCommand command)
    {
        var editorApi =
            editorState.Value.EditorApi
            ?? throw new InvalidOperationException("Editor API is not initialized");

        // clear selection before deleting node
        dispatcher.Dispatch(new ChangeSelectionCommand(editorState.Value.CanvasId, []));

        return await editorApi.DeleteNodeAsync(
            new ModelEntityCommand() { Id = command.NodeId, ModelId = command.ModelId }
        );
    }

    protected override async ValueTask<Result<ModelEntityResponse>> UpdateServer(
        DeleteNodeClientCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClientV1.DeleteNodeAsync(
            command.NodeId,
            command.ModelId,
            ct
        );
    }

    protected override async ValueTask<Result> UpdateEditorAfterServerResponse(
        DeleteNodeClientCommand command,
        Result<ModelEntityResponse> serverResponse
    )
    {
        if (serverResponse.IsError)
        {
            var editorApi =
                editorState.Value.EditorApi
                ?? throw new InvalidOperationException("Editor API is not initialized");
            return await editorApi.CreateNodeAsync(
                new NodeResponse(command.NodeId, command.ModelId, command.Data).ToEditorUnits()
            );
        }

        return Result.Success;
    }

    protected override ValueTask<Result> UpdateClient(
        DeleteNodeClientCommand command,
        Result<ModelEntityResponse> serverResponse
    )
    {
        if (serverResponse.IsSuccess)
        {
            dispatcher.Dispatch(command);
        }

        return new(Result.Success);
    }
}

public sealed class DeleteNodeSimpleCommandHandler(
    DeleteNodeCommandHandler deleteNodeCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<ModelEntityCommand, DeleteNodeClientCommand, ModelEntityResponse>(
        deleteNodeCommandHandler
    )
{
    protected override DeleteNodeClientCommand CreateCommand(ModelEntityCommand simpleCommand)
    {
        var node =
            (editorState.Value.CachedModelResponse?.Nodes.GetValueOrDefault(simpleCommand.Id))
            ?? throw new InvalidOperationException("Node not found in editor state");

        return new()
        {
            ModelId = simpleCommand.ModelId,
            NodeId = simpleCommand.Id,
            Data = node.ToNodeData(),
        };
    }
}
