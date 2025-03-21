using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.Nodes;

public sealed class DeleteNodeCommandHandler(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IState<EditorComponentState> editorState,
    IDispatcher dispatcher,
    ISnackbar snackbar
) : CommandHandlerBase<ModelEntityCommand, ModelEntityResponse>(snackbar)
{
    protected override async Task<Result<ModelEntityResponse>> ExecuteCommandAsync(
        ModelEntityCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClientV1.DeleteNodeAsync(command.ModelId, command.Id, ct);
    }

    protected override void PostProcess(
        ModelEntityCommand command,
        Result<ModelEntityResponse> response
    )
    {
        if (response.IsSuccess)
        {
            var cachedModel = editorState.Value.CachedModelResponse;
            var currentNodeValue = cachedModel?.Nodes.GetValueOrDefault(command.Id)
                ?? throw new InvalidOperationException("Node not found in cache");

            dispatcher.Dispatch(
                new DeleteNodeClientCommand(currentNodeValue)
                {
                    HandledByServer = true
                }
            );
        }
    }
}
