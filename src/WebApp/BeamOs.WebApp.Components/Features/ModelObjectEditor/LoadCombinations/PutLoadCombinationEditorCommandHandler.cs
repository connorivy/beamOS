using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.LoadCombinations;

public sealed class PutLoadCombinationEditorCommandHandler(
    ILogger<PutLoadCombinationEditorCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher
) : ClientCommandHandlerBase<PutLoadCombinationClientCommand, LoadCombination>(logger, snackbar)
{
    protected override async ValueTask<Result<LoadCombination>> UpdateServer(
        PutLoadCombinationClientCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClientV1.PutLoadCombination(
            command.ModelId,
            command.New.Id,
            command.New,
            ct
        );
    }

    protected override ValueTask<Result> UpdateClient(
        PutLoadCombinationClientCommand command,
        Result<LoadCombination> serverResponse
    )
    {
        if (serverResponse.IsSuccess)
        {
            dispatcher.Dispatch(command);
        }

        return new(Result.Success);
    }
}

public sealed class PutLoadCombinationSimpleCommandHandler(
    PutLoadCombinationEditorCommandHandler putLoadCombinationEditorCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<LoadCombination, PutLoadCombinationClientCommand, LoadCombination>(
        putLoadCombinationEditorCommandHandler
    )
{
    protected override PutLoadCombinationClientCommand CreateCommand(LoadCombination simpleCommand)
    {
        var sectionProfile =
            (
                editorState.Value.CachedModelResponse?.LoadCombinations.GetValueOrDefault(
                    simpleCommand.Id
                )
            ) ?? throw new InvalidOperationException("Section profile not found in editor state");

        return new(editorState.Value.CachedModelResponse.Id, sectionProfile, simpleCommand);
    }
}
