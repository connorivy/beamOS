using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.LoadCombinations;

public sealed class DeleteLoadCombinationCommandHandler(
    ILogger<DeleteLoadCombinationCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState
)
    : ClientCommandHandlerBase<DeleteLoadCombinationClientCommand, ModelEntityResponse>(
        logger,
        snackbar
    )
{
    protected override async ValueTask<Result<ModelEntityResponse>> UpdateServer(
        DeleteLoadCombinationClientCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClientV1.DeleteLoadCombination(
            command.ModelId,
            command.LoadCombinationId,
            ct
        );
    }

    protected override ValueTask<Result> UpdateClient(
        DeleteLoadCombinationClientCommand command,
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

public sealed class DeleteLoadCombinationSimpleCommandHandler(
    DeleteLoadCombinationCommandHandler deleteLoadCombinationCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<
        ModelEntityCommand,
        DeleteLoadCombinationClientCommand,
        ModelEntityResponse
    >(deleteLoadCombinationCommandHandler)
{
    protected override DeleteLoadCombinationClientCommand CreateCommand(
        ModelEntityCommand simpleCommand
    )
    {
        var loadCombination =
            (
                editorState.Value.CachedModelResponse?.LoadCombinations.GetValueOrDefault(
                    simpleCommand.Id
                )
            ) ?? throw new InvalidOperationException("Section profile not found in editor state");

        return new()
        {
            ModelId = simpleCommand.ModelId,
            LoadCombinationId = simpleCommand.Id,
            Data = loadCombination,
        };
    }
}
