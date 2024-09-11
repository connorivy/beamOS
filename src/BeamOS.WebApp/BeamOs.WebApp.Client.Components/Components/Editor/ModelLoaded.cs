using BeamOs.WebApp.Client.Components.Features.StructuralApiClient;
using Fluxor;

namespace BeamOs.WebApp.Client.Components.Components.Editor;

public readonly record struct ModelLoaded(string ModelId);

public static class ModelLoadedReducer
{
    [ReducerMethod]
    public static StructuralApiClientState ReduceModelLoadedAction(
        StructuralApiClientState state,
        ModelLoaded action
    )
    {
        return state with { ModelId = action.ModelId };
    }
}
