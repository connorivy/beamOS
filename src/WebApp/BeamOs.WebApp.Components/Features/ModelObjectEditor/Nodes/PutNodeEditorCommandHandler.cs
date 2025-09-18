using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Riok.Mapperly.Abstractions;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.Nodes;

public sealed class PutNodeEditorCommandHandler(
    ILogger<PutNodeEditorCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState
) : ClientCommandHandlerBase<PutNodeClientCommand, NodeResponse>(logger, snackbar)
{
    protected override async ValueTask<Result> UpdateEditor(PutNodeClientCommand command)
    {
        var editorApi =
            editorState.Value.EditorApi
            ?? throw new InvalidOperationException("Editor API is not initialized");

        return await editorApi.UpdateNodeAsync(command.New.ToEditorUnits());
    }

    protected override async ValueTask<Result<NodeResponse>> UpdateServer(
        PutNodeClientCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClientV1.PutNodeAsync(
            command.New.ModelId,
            command.New.Id,
            command.New.ToNodeData(),
            ct
        );
    }

    protected override async ValueTask<Result> UpdateEditorAfterServerResponse(
        PutNodeClientCommand command,
        Result<NodeResponse> serverResponse
    )
    {
        if (serverResponse.IsError)
        {
            var editorApi =
                editorState.Value.EditorApi
                ?? throw new InvalidOperationException("Editor API is not initialized");

            return await editorApi.UpdateNodeAsync(command.Previous.ToEditorUnits());
        }
        else
        {
            // todo: do I need to update with the server response?
            // it should be the same as the command value
        }

        return Result.Success;
    }

    protected override ValueTask<Result> UpdateClient(
        PutNodeClientCommand command,
        Result<NodeResponse> serverResponse
    )
    {
        if (serverResponse.IsSuccess)
        {
            dispatcher.Dispatch(command);
        }

        return new(Result.Success);
    }
}

public sealed class PutNodeSimpleCommandHandler(
    PutNodeEditorCommandHandler putNodeEditorCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<
        ModelResourceWithIntIdRequest<NodeData>,
        PutNodeClientCommand,
        NodeResponse
    >(putNodeEditorCommandHandler)
{
    protected override PutNodeClientCommand CreateCommand(
        ModelResourceWithIntIdRequest<NodeData> simpleCommand
    )
    {
        var node =
            (editorState.Value.CachedModelResponse?.Nodes.GetValueOrDefault(simpleCommand.Id))
            ?? throw new InvalidOperationException("Node not found in editor state");

        return new(node, simpleCommand.ToResponse());
    }
}

[Mapper]
internal static partial class NodeDataMapper
{
    [MapNestedProperties(nameof(ModelResourceWithIntIdRequest<>.Body))]
    public static partial NodeResponse ToResponse(
        this ModelResourceWithIntIdRequest<NodeData> request
    );
}
