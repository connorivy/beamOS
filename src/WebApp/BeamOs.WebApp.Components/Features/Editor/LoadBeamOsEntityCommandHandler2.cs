using BeamOs.CodeGen.EditorApi;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.Editor;

public class LoadBeamOsEntityCommandHandler2(
    ISnackbar snackbar,
    ILogger<LoadBeamOsEntityCommandHandler2> logger
) : ClientCommandHandlerBase<LoadBeamOsEntityCommand, bool>(logger, snackbar)
{
    protected override ValueTask<Result> UpdateEditor(LoadBeamOsEntityCommand command)
    {
        return command.EntityResponse switch
        {
            ModelResponse modelResponse => DisplayModelResponse(
                modelResponse,
                command.EditorApi,
                CancellationToken.None
            ),
            ModelProposalResponse modelProposalResponse => DisplayModelProposal(
                modelProposalResponse,
                command.EditorApi,
                CancellationToken.None
            ),
            _ => ValueTask.FromResult<Result>(
                BeamOsError.Failure(
                    description: $"Unsupported entity type: {command.EntityResponse.GetType()}"
                )
            ),
        };
    }

    private static async ValueTask<Result> DisplayModelResponse(
        ModelResponse modelResponse,
        IEditorApiAlpha editorApi,
        CancellationToken ct
    )
    {
        await editorApi.ClearAsync(ct);

        await editorApi.SetSettingsAsync(modelResponse.Settings, ct);

        await editorApi.CreateModelAsync(modelResponse.ToEditorUnits(), ct);

        return Result.Success;
    }

    private static async ValueTask<Result> DisplayModelProposal(
        ModelProposalResponse entity,
        IEditorApiAlpha editorApi,
        CancellationToken ct
    )
    {
        await editorApi.DisplayModelProposalAsync(entity, ct);
        return Result.Success;
    }
}

public record struct LoadBeamOsEntityCommand(
    IBeamOsEntityResponse EntityResponse,
    IEditorApiAlpha EditorApi
) : ILoadEntityResponseCommand, IBeamOsUndoableClientCommand
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }

    public IBeamOsUndoableClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        throw new NotImplementedException();

    public IBeamOsUndoableClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };
}
