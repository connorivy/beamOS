using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor;
public partial class ModelObjectEditor(IState<ModelObjectEditorState> state, IDispatcher dispatcher) : FluxorComponent
{
    [Parameter]
    public required UnitSettingsContract UnitSettings { get; set; }

    [Parameter]
    public required Guid ModelId { get; set; }

    private void ShowView(BeamOsObjectType viewType)
    {
        dispatcher.Dispatch(new ShowView(viewType));
    }

    private void GoBack()
    {
        dispatcher.Dispatch(new ShowView(BeamOsObjectType.Model));
    }

}

[FeatureState]
public record ModelObjectEditorState(BeamOsObjectType CurrentViewType, SelectedObject? SelectedObject)
{
    public ModelObjectEditorState() : this(BeamOsObjectType.Model, null)
    {
    }
}

public static class Reducers
{
    [ReducerMethod]
    public static ModelObjectEditorState Reducer(ModelObjectEditorState state, ChangeSelectionCommand action)
    {
        var selectedObject = action.SelectedObjects?.FirstOrDefault();
        var objectType = selectedObject?.TypeName switch
        {
            "Node" => BeamOsObjectType.Node,
            _ => BeamOsObjectType.Model,
        };
        return state with { CurrentViewType = objectType, SelectedObject = action.SelectedObjects?.FirstOrDefault() };
    }

    [ReducerMethod]
    public static ModelObjectEditorState Reducer(ModelObjectEditorState state, ShowView action) => state with { CurrentViewType = action.BeamOsObjectType };
}

public record struct ShowView(BeamOsObjectType BeamOsObjectType);
