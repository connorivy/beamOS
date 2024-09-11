using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Client.Components.Features.StructuralApiClient;

public readonly record struct ElementReferencesEvaluated(List<ElementReference> ElementReferences);

public static class ElementReferencesEvaluatedActionReducer
{
    [ReducerMethod]
    public static StructuralApiClientState ReduceElementReferencesEvaluated(
        StructuralApiClientState state,
        ElementReferencesEvaluated action
    )
    {
        return state with { LazyElementRefs = null, ElementRefs = action.ElementReferences };
    }
}
