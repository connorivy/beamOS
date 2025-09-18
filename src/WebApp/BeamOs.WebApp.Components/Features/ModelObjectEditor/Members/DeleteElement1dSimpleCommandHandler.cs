using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.ModelObjectEditor.MomentLoads;
using Fluxor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.Members;

public sealed class DeleteElement1dSimpleCommandHandler(
    DeleteElement1dCommandHandler deleteElement1dCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<
        ModelEntityCommand,
        DeleteElement1dClientCommand,
        ModelEntityResponse
    >(deleteElement1dCommandHandler)
{
    protected override DeleteElement1dClientCommand CreateCommand(ModelEntityCommand simpleCommand)
    {
        var element1d =
            editorState.Value.CachedModelResponse?.Element1ds.GetValueOrDefault(simpleCommand.Id)
            ?? throw new InvalidOperationException("Element1d not found in editor state");

        return new()
        {
            ModelId = simpleCommand.ModelId,
            Element1dId = simpleCommand.Id,
            Data = element1d.ToElement1dData(),
        };
    }
}
