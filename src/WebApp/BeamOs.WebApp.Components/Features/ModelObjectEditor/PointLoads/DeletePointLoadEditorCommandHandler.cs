using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.PointLoads;

public sealed class DeletePointLoadEditorCommandHandler(
    ILogger<DeletePointLoadEditorCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState
) : ClientCommandHandlerBase<DeletePointLoadClientCommand, ModelEntityResponse>(logger, snackbar)
{
    protected override async ValueTask<Result> UpdateEditor(DeletePointLoadClientCommand command)
    {
        var editorApi =
            editorState.Value.EditorApi
            ?? throw new InvalidOperationException("Editor API is not initialized");

        return await editorApi.DeletePointLoadAsync(
            new ModelEntityCommand() { Id = command.PointLoadId, ModelId = command.ModelId }
        );
    }

    protected override async ValueTask<Result<ModelEntityResponse>> UpdateServer(
        DeletePointLoadClientCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClientV1.DeletePointLoadAsync(
            command.ModelId,
            command.PointLoadId,
            ct
        );
    }

    protected override async ValueTask<Result> UpdateEditorAfterServerResponse(
        DeletePointLoadClientCommand command,
        Result<ModelEntityResponse> serverResponse
    )
    {
        if (serverResponse.IsError)
        {
            var editorApi =
                editorState.Value.EditorApi
                ?? throw new InvalidOperationException("Editor API is not initialized");

            return await editorApi.CreatePointLoadAsync(
                new(
                    command.PointLoadId,
                    command.Data.NodeId,
                    command.Data.LoadCaseId,
                    command.ModelId,
                    command.Data.Force,
                    command.Data.Direction
                )
            );
        }

        return Result.Success;
    }

    protected override ValueTask<Result> UpdateClient(
        DeletePointLoadClientCommand command,
        Result<ModelEntityResponse> serverResponse
    )
    {
        // Nothing to do here since we're deleting the entity
        return new(Result.Success);
    }
}

public record DeletePointLoadClientCommand : IBeamOsClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }
    public required Guid ModelId { get; init; }
    public int PointLoadId { get; init; }
    public required PointLoadData Data { get; init; }

    public IBeamOsClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new CreatePointLoadClientCommand(this.Data)
        {
            ModelId = this.ModelId,
            PointLoadId = this.PointLoadId,
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };

    public IBeamOsClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };
}

public sealed class DeletePointLoadSimpleCommandHandler(
    DeletePointLoadEditorCommandHandler deletePointLoadEditorCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<
        ModelEntityCommand,
        DeletePointLoadClientCommand,
        ModelEntityResponse
    >(deletePointLoadEditorCommandHandler)
{
    protected override DeletePointLoadClientCommand CreateCommand(ModelEntityCommand simpleCommand)
    {
        var pointLoad =
            (editorState.Value.CachedModelResponse?.PointLoads.GetValueOrDefault(simpleCommand.Id))
            ?? throw new InvalidOperationException("PointLoad not found in editor state");

        return new DeletePointLoadClientCommand
        {
            PointLoadId = pointLoad.Id,
            ModelId = simpleCommand.ModelId,
            Data = pointLoad,
        };
    }
}
