using Fluxor;

namespace BeamOs.WebApp.Client.Components.Features.StructuralApiClient;

public readonly struct CurrentFieldDeselected();

//public static class FieldDeselectedActionReducer
//{
//    [ReducerMethod(typeof(CurrentFieldDeselected))]
//    public static StructuralApiClientState ReduceFieldDeselectedAction(
//        StructuralApiClientState state
//    )
//    {
//        return state with
//        {
//            CurrentlySelectedFieldIndex = null,
//            PreviouslySelectedFieldIndex = state.CurrentlySelectedFieldIndex
//        };
//    }
//}
