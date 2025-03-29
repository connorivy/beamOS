using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.Members;

public sealed class DeleteElement1dCommandHandler(
    ILogger<DeleteElement1dCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState
) : ClientCommandHandlerBase<DeleteElement1dClientCommand, ModelEntityResponse>(logger, snackbar)
{
    protected override async ValueTask<Result> UpdateEditor(DeleteElement1dClientCommand command)
    {
        var editorApi =
            editorState.Value.EditorApi
            ?? throw new InvalidOperationException("Editor API is not initialized");

        // clear selection before deleting element
        dispatcher.Dispatch(new ChangeSelectionCommand(editorState.Value.CanvasId, []));

        return await editorApi.DeleteElement1dAsync(
            new ModelEntityCommand() { Id = command.Element1dId, ModelId = command.ModelId }
        );
    }

    protected override async ValueTask<Result<ModelEntityResponse>> UpdateServer(
        DeleteElement1dClientCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClientV1.DeleteElement1dAsync(
            command.ModelId,
            command.Element1dId,
            ct
        );
    }

    protected override async ValueTask<Result> UpdateEditorAfterServerResponse(
        DeleteElement1dClientCommand command,
        Result<ModelEntityResponse> serverResponse
    )
    {
        if (serverResponse.IsError)
        {
            var editorApi =
                editorState.Value.EditorApi
                ?? throw new InvalidOperationException("Editor API is not initialized");
            return await editorApi.CreateElement1dAsync(
                new Element1dResponse(
                    command.Element1dId,
                    command.ModelId,
                    command.Data.StartNodeId,
                    command.Data.EndNodeId,
                    command.Data.MaterialId,
                    command.Data.SectionProfileId,
                    command.Data.SectionProfileRotation.Value,
                    null
                )
            );
        }

        return Result.Success;
    }

    protected override ValueTask<Result> UpdateClient(
        DeleteElement1dClientCommand command,
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

public record DeleteElement1dClientCommand : IBeamOsClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }
    public required Guid ModelId { get; init; }
    public int Element1dId { get; init; }
    public required Element1dData Data { get; init; }

    public IBeamOsClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new CreateElement1dClientCommand(this.Data)
        {
            ModelId = this.ModelId,
            Element1dId = this.Element1dId,
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
