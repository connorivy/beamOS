namespace BeamOs.WebApp.Client.Components.Components.Editor;

public readonly struct CanvasClicked();

//public static class CanvasClickedReducer
//{
//    [ReducerMethod(typeof(CanvasClicked))]
//    public static StructuralApiClientState ReduceCanvasClickedAction(StructuralApiClientState state)
//    {
//        if (
//            state.CurrentlySelectedFieldIndex is null
//            && state.PreviouslySelectedFieldIndex is not null
//        )
//        {
//            return state with { CurrentlySelectedFieldIndex = state.PreviouslySelectedFieldIndex };
//        }
//        return state;
//    }
//}
