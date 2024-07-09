using BeamOs.WebApp.Client.Actions.EditorActions;
using Fluxor;
using static BeamOS.WebApp.Client.Components.Editor.EditorComponent;

namespace BeamOS.WebApp.Client.Components.Editor.Flux.Reducers;

public static class EditorEventReducers
{
    [ReducerMethod]
    public static EditorComponentState ReduceSelectionChangedEvent(
        EditorComponentState state,
        ChangeSelectionAction selectionChangedEvent
    ) => state with { SelectedObjects = selectionChangedEvent.SelectedObjects };
}
