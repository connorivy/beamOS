using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.ModelObjectEditor.MomentLoads;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.Materials;

public sealed class DeleteMaterialCommandHandler(
    ILogger<DeleteMaterialCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState
) : ClientCommandHandlerBase<DeleteMaterialClientCommand, ModelEntityResponse>(logger, snackbar)
{
    protected override ValueTask<Result<ModelEntityResponse>> UpdateServer(
        DeleteMaterialClientCommand command,
        CancellationToken ct = default
    )
    {
        // todo
        // return await structuralAnalysisApiClientV1.DeleteMaterialAsync(
        //     command.ModelId,
        //     command.MaterialId,
        //     ct
        // );
        return ValueTask.FromResult(Result<ModelEntityResponse>.Success());
    }

    protected override ValueTask<Result> UpdateClient(
        DeleteMaterialClientCommand command,
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

public sealed class DeleteMaterialSimpleCommandHandler(
    DeleteMaterialCommandHandler deleteMaterialCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<
        ModelEntityCommand,
        DeleteMaterialClientCommand,
        ModelEntityResponse
    >(deleteMaterialCommandHandler)
{
    protected override DeleteMaterialClientCommand CreateCommand(ModelEntityCommand simpleCommand)
    {
        var sectionProfile =
            (editorState.Value.CachedModelResponse?.Materials.GetValueOrDefault(simpleCommand.Id))
            ?? throw new InvalidOperationException("Section profile not found in editor state");

        return new()
        {
            ModelId = simpleCommand.ModelId,
            MaterialId = simpleCommand.Id,
            Data = sectionProfile.ToMaterialData(),
        };
    }
}
