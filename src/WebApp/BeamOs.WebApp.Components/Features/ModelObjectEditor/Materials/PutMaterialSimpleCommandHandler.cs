using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.ModelObjectEditor.Materials;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.Materials;

public sealed class PutMaterialClientCommandHandler(
    ILogger<PutMaterialSimpleCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher
) : ClientCommandHandlerBase<PutMaterialClientCommand, MaterialResponse>(logger, snackbar)
{
    protected override async ValueTask<Result<MaterialResponse>> UpdateServer(
        PutMaterialClientCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClientV1.PutMaterial(
            command.New.Id,
            command.New.ModelId,
            command.New.ToMaterialData(),
            ct
        );
    }

    protected override ValueTask<Result> UpdateClient(
        PutMaterialClientCommand command,
        Result<MaterialResponse> serverResponse
    )
    {
        if (serverResponse.IsSuccess)
        {
            dispatcher.Dispatch(command);
        }

        return new(Result.Success);
    }
}

public sealed class PutMaterialSimpleCommandHandler(
    PutMaterialClientCommandHandler putMaterialEditorCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<PutMaterialCommand, PutMaterialClientCommand, MaterialResponse>(
        putMaterialEditorCommandHandler
    )
{
    protected override PutMaterialClientCommand CreateCommand(PutMaterialCommand simpleCommand)
    {
        var sectionProfile =
            (editorState.Value.CachedModelResponse?.Materials.GetValueOrDefault(simpleCommand.Id))
            ?? throw new InvalidOperationException("Section profile not found in editor state");

        return new(sectionProfile, simpleCommand.ToResponse());
    }
}

public record PutMaterialClientCommand(MaterialResponse Previous, MaterialResponse New)
    : IBeamOsUndoableClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();

    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }

    public IBeamOsUndoableClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new PutMaterialClientCommand(this.New, this.Previous)
        {
            HandledByEditor = this.HandledByEditor,
            HandledByBlazor = this.HandledByBlazor,
            HandledByServer = this.HandledByServer,
        };

    public IBeamOsUndoableClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };
}
