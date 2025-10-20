using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.MomentLoads;

public sealed class DeleteMomentLoadCommandHandler(
    ILogger<DeleteMomentLoadCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState
) : ClientCommandHandlerBase<DeleteMomentLoadClientCommand, ModelEntityResponse>(logger, snackbar)
{
    // TODO : show moment loads in editor
    // protected override async ValueTask<Result> UpdateEditor(DeleteMomentLoadClientCommand command)
    // {
    //     var editorApi =
    //         editorState.Value.EditorApi
    //         ?? throw new InvalidOperationException("Editor API is not initialized");

    //     // clear selection before deleting node
    //     dispatcher.Dispatch(new ChangeSelectionCommand(editorState.Value.CanvasId, []));

    //     return await editorApi.DeleteMomentLoadAsync(
    //         new ModelEntityCommand() { Id = command.MomentLoadId, ModelId = command.ModelId }
    //     );
    // }

    protected override async ValueTask<Result<ModelEntityResponse>> UpdateServer(
        DeleteMomentLoadClientCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClientV1.DeleteMomentLoadAsync(
            command.MomentLoadId,
            command.ModelId,
            ct
        );
    }

    // protected override async ValueTask<Result> UpdateEditorAfterServerResponse(
    //     DeleteMomentLoadClientCommand command,
    //     Result<ModelEntityResponse> serverResponse
    // )
    // {
    //     if (serverResponse.IsError)
    //     {
    //         var editorApi =
    //             editorState.Value.EditorApi
    //             ?? throw new InvalidOperationException("Editor API is not initialized");
    //         return await editorApi.CreateMomentLoadAsync(
    //             new MomentLoadResponse(command.MomentLoadId, command.ModelId, command.Data).ToEditorUnits()
    //         );
    //     }

    //     return Result.Success;
    // }

    protected override ValueTask<Result> UpdateClient(
        DeleteMomentLoadClientCommand command,
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

public sealed class DeleteMomentLoadSimpleCommandHandler(
    DeleteMomentLoadCommandHandler deleteMomentLoadCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<
        ModelEntityCommand,
        DeleteMomentLoadClientCommand,
        ModelEntityResponse
    >(deleteMomentLoadCommandHandler)
{
    protected override DeleteMomentLoadClientCommand CreateCommand(ModelEntityCommand simpleCommand)
    {
        var node =
            (editorState.Value.CachedModelResponse?.MomentLoads.GetValueOrDefault(simpleCommand.Id))
            ?? throw new InvalidOperationException("MomentLoad not found in editor state");

        return new()
        {
            ModelId = simpleCommand.ModelId,
            MomentLoadId = simpleCommand.Id,
            Data = node.ToData(),
        };
    }
}

public record DeleteMomentLoadClientCommand : IBeamOsUndoableClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }
    public required Guid ModelId { get; init; }
    public int MomentLoadId { get; init; }
    public required MomentLoadData Data { get; init; }

    public IBeamOsUndoableClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new CreateMomentLoadClientCommand(this.Data)
        {
            ModelId = this.ModelId,
            MomentLoadId = this.MomentLoadId,
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };

    public IBeamOsUndoableClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };
}

public readonly struct ModelEntityCommand : IModelEntity
{
    public Guid ModelId { get; init; }
    public int Id { get; init; }
}
