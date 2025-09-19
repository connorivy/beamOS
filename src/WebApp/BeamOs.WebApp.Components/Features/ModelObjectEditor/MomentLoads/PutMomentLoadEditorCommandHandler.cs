using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Riok.Mapperly.Abstractions;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.MomentLoads;

public sealed class PutMomentLoadEditorCommandHandler(
    ILogger<PutMomentLoadEditorCommandHandler> logger,
    ISnackbar snackbar,
    BeamOsResultApiClient apiClient,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState
) : ClientCommandHandlerBase<PutMomentLoadClientCommand, MomentLoadResponse>(logger, snackbar)
{
    // protected override async ValueTask<Result> UpdateEditor(PutMomentLoadClientCommand command)
    // {
    //     var editorApi =
    //         editorState.Value.EditorApi
    //         ?? throw new InvalidOperationException("Editor API is not initialized");

    //     return await editorApi.UpdateMomentLoadAsync(command.New.ToEditorUnits());
    // }

    protected override async ValueTask<Result<MomentLoadResponse>> UpdateServer(
        PutMomentLoadClientCommand command,
        CancellationToken ct = default
    )
    {
        return await apiClient
            .Models[command.New.ModelId]
            .MomentLoads[command.New.Id]
            .PutMomentLoadAsync(command.New.ToData(), ct);
    }

    // protected override async ValueTask<Result> UpdateEditorAfterServerResponse(
    //     PutMomentLoadClientCommand command,
    //     Result<MomentLoadResponse> serverResponse
    // )
    // {
    //     if (serverResponse.IsError)
    //     {
    //         var editorApi =
    //             editorState.Value.EditorApi
    //             ?? throw new InvalidOperationException("Editor API is not initialized");

    //         return await editorApi.UpdateMomentLoadAsync(command.Previous.ToEditorUnits());
    //     }
    //     else
    //     {
    //         // todo: do I need to update with the server response?
    //         // it should be the same as the command value
    //     }

    //     return Result.Success;
    // }

    protected override ValueTask<Result> UpdateClient(
        PutMomentLoadClientCommand command,
        Result<MomentLoadResponse> serverResponse
    )
    {
        if (serverResponse.IsSuccess)
        {
            dispatcher.Dispatch(command);
        }

        return new(Result.Success);
    }
}

public sealed class PutMomentLoadSimpleCommandHandler(
    PutMomentLoadEditorCommandHandler putMomentLoadEditorCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<
        ModelResourceWithIntIdRequest<MomentLoadData>,
        PutMomentLoadClientCommand,
        MomentLoadResponse
    >(putMomentLoadEditorCommandHandler)
{
    protected override PutMomentLoadClientCommand CreateCommand(
        ModelResourceWithIntIdRequest<MomentLoadData> simpleCommand
    )
    {
        var node =
            (editorState.Value.CachedModelResponse?.MomentLoads.GetValueOrDefault(simpleCommand.Id))
            ?? throw new InvalidOperationException("MomentLoad not found in editor state");

        return new(node, simpleCommand.ToResponse());
    }
}

public record PutMomentLoadClientCommand(MomentLoadResponse Previous, MomentLoadResponse New)
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

[Mapper]
internal static partial class PutMomentLoadCommandMapper
{
    [MapNestedProperties(nameof(ModelResourceWithIntIdRequest<>.Body))]
    public static partial MomentLoadResponse ToResponse(
        this ModelResourceWithIntIdRequest<MomentLoadData> command
    );
}
