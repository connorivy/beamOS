using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.PointLoads;

public sealed class PutPointLoadEditorCommandHandler(
    ILogger<PutPointLoadEditorCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState
) : ClientCommandHandlerBase<PutPointLoadClientCommand, PointLoadResponse>(logger, snackbar)
{
    protected override async ValueTask<Result> UpdateEditor(PutPointLoadClientCommand command)
    {
        var editorApi =
            editorState.Value.EditorApi
            ?? throw new InvalidOperationException("Editor API is not initialized");

        var deleteCommand = await editorApi.DeletePointLoadAsync(command.Previous);
        if (deleteCommand.IsError)
        {
            return deleteCommand;
        }
        return await editorApi.CreatePointLoadAsync(command.New.ToEditorUnits());
    }

    protected override async ValueTask<Result<PointLoadResponse>> UpdateServer(
        PutPointLoadClientCommand command,
        CancellationToken ct = default
    )
    {
        var pointLoadData = new PointLoadData(
            command.New.NodeId,
            command.New.LoadCaseId,
            command.New.Force,
            command.New.Direction
        );
        return await structuralAnalysisApiClientV1.PutPointLoadAsync(
            command.New.Id,
            command.New.ModelId,
            pointLoadData,
            ct
        );
    }

    protected override async ValueTask<Result> UpdateEditorAfterServerResponse(
        PutPointLoadClientCommand command,
        Result<PointLoadResponse> serverResponse
    )
    {
        if (serverResponse.IsError)
        {
            var editorApi =
                editorState.Value.EditorApi
                ?? throw new InvalidOperationException("Editor API is not initialized");

            var deleteCommand = await editorApi.DeletePointLoadAsync(command.New);
            if (deleteCommand.IsError)
            {
                return deleteCommand;
            }
            return await editorApi.CreatePointLoadAsync(command.Previous.ToEditorUnits());
        }

        return Result.Success;
    }

    protected override ValueTask<Result> UpdateClient(
        PutPointLoadClientCommand command,
        Result<PointLoadResponse> serverResponse
    )
    {
        if (serverResponse.IsSuccess)
        {
            dispatcher.Dispatch(command);
        }

        return new(Result.Success);
    }
}

public sealed class PutPointLoadSimpleCommandHandler(
    PutPointLoadEditorCommandHandler putPointLoadEditorCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<PutPointLoadCommand, PutPointLoadClientCommand, PointLoadResponse>(
        putPointLoadEditorCommandHandler
    )
{
    protected override PutPointLoadClientCommand CreateCommand(PutPointLoadCommand simpleCommand)
    {
        var pointLoad =
            (editorState.Value.CachedModelResponse?.PointLoads.GetValueOrDefault(simpleCommand.Id))
            ?? throw new InvalidOperationException("PointLoad not found in editor state");

        return new(pointLoad, simpleCommand.ToResponse());
    }
}

public static class PutPointLoadCommandExtensions
{
    public static PointLoadResponse ToResponse(this PutPointLoadCommand command)
    {
        return new PointLoadResponse(
            command.Id,
            command.NodeId,
            command.LoadCaseId,
            command.ModelId,
            command.Force,
            new BeamOs.StructuralAnalysis.Contracts.Common.Vector3(
                command.Direction.X,
                command.Direction.Y,
                command.Direction.Z
            )
        );
    }

    public static PointLoadResponse ToEditorUnits(this PointLoadResponse response)
    {
        return response with
        {
            Force = new(
                response.Force.As(ForceUnitContract.Kilonewton),
                ForceUnitContract.Kilonewton
            ),
        };
    }
}

public record PutPointLoadClientCommand(PointLoadResponse Previous, PointLoadResponse New)
    : IBeamOsUndoableClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }

    public virtual IBeamOsUndoableClientCommand GetUndoCommand(
        BeamOsClientCommandArgs? args = null
    ) =>
        this with
        {
            New = this.Previous,
            Previous = this.New,
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };

    public virtual IBeamOsUndoableClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };
}
