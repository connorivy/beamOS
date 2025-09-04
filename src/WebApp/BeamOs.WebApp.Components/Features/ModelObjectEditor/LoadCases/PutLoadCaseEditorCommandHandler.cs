using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.LoadCases;

public sealed class PutLoadCaseEditorCommandHandler(
    ILogger<PutLoadCaseEditorCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher
) : ClientCommandHandlerBase<PutLoadCaseClientCommand, LoadCase>(logger, snackbar)
{
    protected override async ValueTask<Result<LoadCase>> UpdateServer(
        PutLoadCaseClientCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClientV1.PutLoadCase(
            command.ModelId,
            command.New.Id,
            command.New,
            ct
        );
    }

    protected override ValueTask<Result> UpdateClient(
        PutLoadCaseClientCommand command,
        Result<LoadCase> serverResponse
    )
    {
        if (serverResponse.IsSuccess)
        {
            dispatcher.Dispatch(command);
        }

        return new(Result.Success);
    }
}

public sealed class PutLoadCaseSimpleCommandHandler(
    PutLoadCaseEditorCommandHandler putLoadCaseEditorCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<LoadCase, PutLoadCaseClientCommand, LoadCase>(
        putLoadCaseEditorCommandHandler
    )
{
    protected override PutLoadCaseClientCommand CreateCommand(LoadCase simpleCommand)
    {
        var sectionProfile =
            (editorState.Value.CachedModelResponse?.LoadCases.GetValueOrDefault(simpleCommand.Id))
            ?? throw new InvalidOperationException("Section profile not found in editor state");

        return new(editorState.Value.CachedModelResponse.Id, sectionProfile, simpleCommand);
    }
}
