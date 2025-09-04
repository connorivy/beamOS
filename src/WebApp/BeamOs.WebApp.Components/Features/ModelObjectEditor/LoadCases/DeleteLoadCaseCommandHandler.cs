using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.LoadCases;

public sealed class DeleteLoadCaseCommandHandler(
    ILogger<DeleteLoadCaseCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState
) : ClientCommandHandlerBase<DeleteLoadCaseClientCommand, ModelEntityResponse>(logger, snackbar)
{
    protected override async ValueTask<Result<ModelEntityResponse>> UpdateServer(
        DeleteLoadCaseClientCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClientV1.DeleteLoadCaseAsync(
            command.ModelId,
            command.LoadCaseId,
            ct
        );
    }

    protected override ValueTask<Result> UpdateClient(
        DeleteLoadCaseClientCommand command,
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

public sealed class DeleteLoadCaseSimpleCommandHandler(
    DeleteLoadCaseCommandHandler deleteLoadCaseCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<
        ModelEntityCommand,
        DeleteLoadCaseClientCommand,
        ModelEntityResponse
    >(deleteLoadCaseCommandHandler)
{
    protected override DeleteLoadCaseClientCommand CreateCommand(ModelEntityCommand simpleCommand)
    {
        var loadCase =
            (editorState.Value.CachedModelResponse?.LoadCases.GetValueOrDefault(simpleCommand.Id))
            ?? throw new InvalidOperationException("Section profile not found in editor state");

        return new()
        {
            ModelId = simpleCommand.ModelId,
            LoadCaseId = simpleCommand.Id,
            Data = loadCase,
        };
    }
}
