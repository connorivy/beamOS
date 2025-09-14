using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Riok.Mapperly.Abstractions;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.Materials;

public sealed class PutMaterialClientCommandHandler(
    ILogger<PutMaterialSimpleCommandHandler> logger,
    ISnackbar snackbar,
    BeamOsResultApiClient apiClient,
    IDispatcher dispatcher
) : ClientCommandHandlerBase<PutMaterialClientCommand, MaterialResponse>(logger, snackbar)
{
    protected override async ValueTask<Result<MaterialResponse>> UpdateServer(
        PutMaterialClientCommand command,
        CancellationToken ct = default
    )
    {
        return await apiClient
            .Models[command.New.ModelId]
            .Materials[command.New.Id]
            .PutMaterialAsync(command.New.ToMaterialData(), ct);
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
    : SimpleCommandHandlerBase<
        ModelResourceWithIntIdRequest<MaterialData>,
        PutMaterialClientCommand,
        MaterialResponse
    >(putMaterialEditorCommandHandler)
{
    protected override PutMaterialClientCommand CreateCommand(
        ModelResourceWithIntIdRequest<MaterialData> simpleCommand
    )
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

[Mapper]
internal static partial class CreateMaterialStaticMapper
{
    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial MaterialResponse ToResponse(
        this ModelResourceWithIntIdRequest<MaterialData> command
    );
}
