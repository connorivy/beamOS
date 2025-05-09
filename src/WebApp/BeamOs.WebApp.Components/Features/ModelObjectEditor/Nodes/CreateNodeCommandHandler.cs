using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.Nodes;

public sealed class CreateNodeClientCommandHandler(
    ILogger<CreateNodeClientCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState
) : ClientCommandHandlerBase<CreateNodeClientCommand, NodeResponse>(logger, snackbar)
{
    protected override async ValueTask<Result> UpdateEditor(CreateNodeClientCommand command)
    {
        var editorApi =
            editorState.Value.EditorApi
            ?? throw new InvalidOperationException("Editor API is not initialized");

        return await editorApi.CreateNodeAsync(
            new NodeResponse(command.TempNodeId, command.ModelId, command.Data).ToEditorUnits()
        );
    }

    protected override async ValueTask<Result<NodeResponse>> UpdateServer(
        CreateNodeClientCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClientV1.CreateNodeAsync(
            command.ModelId,
            new(command.Data),
            ct
        );
    }

    protected override async ValueTask<Result> UpdateEditorAfterServerResponse(
        CreateNodeClientCommand command,
        Result<NodeResponse> serverResponse
    )
    {
        var editorApi =
            editorState.Value.EditorApi
            ?? throw new InvalidOperationException("Editor API is not initialized");

        if (serverResponse.IsSuccess)
        {
            await editorApi.CreateNodeAsync(serverResponse.Value.ToEditorUnits());
        }

        return await editorApi.DeleteNodeAsync(
            new ModelEntityCommand() { Id = command.TempNodeId, ModelId = command.ModelId }
        );
    }

    protected override ValueTask<Result> UpdateClient(
        CreateNodeClientCommand command,
        Result<NodeResponse> serverResponse
    )
    {
        if (serverResponse.IsSuccess)
        {
            dispatcher.Dispatch(command with { NodeId = serverResponse.Value.Id });
        }

        return new(Result.Success);
    }
}

public record CreateNodeClientCommand(NodeData Data) : IBeamOsClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }
    public int TempNodeId { get; init; } = ClientUtils.GenerateTempId();
    public required Guid ModelId { get; init; }

    /// <summary>
    /// The ID of the node in the model. This is null for temporary nodes.
    /// The ID is generated by the server when the node is created.
    /// </summary>
    public int? NodeId { get; init; }

    public IBeamOsClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new DeleteNodeClientCommand
        {
            ModelId = this.ModelId,
            NodeId = this.NodeId ?? this.TempNodeId,
            Data = this.Data,
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer
        };

    public IBeamOsClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer
        };
}

public record DeleteNodeClientCommand : IBeamOsClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }
    public required Guid ModelId { get; init; }
    public int NodeId { get; init; }
    public required NodeData Data { get; init; }

    public IBeamOsClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new CreateNodeClientCommand(this.Data)
        {
            ModelId = this.ModelId,
            NodeId = this.NodeId,
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer
        };

    public IBeamOsClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer
        };
}
