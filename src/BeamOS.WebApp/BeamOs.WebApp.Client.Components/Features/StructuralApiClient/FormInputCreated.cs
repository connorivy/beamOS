using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Client.Components.Features.StructuralApiClient;

public readonly record struct FormInputCreated(Lazy<ElementReference> LazyReference);

public static class FormInputCreatedActionReducer
{
    [ReducerMethod]
    public static StructuralApiClientState ReduceFormInputCreated(
        StructuralApiClientState state,
        FormInputCreated action
    )
    {
        return state with
        {
            LazyElementRefs = state.LazyElementRefs is null
                ? [action.LazyReference]
                : new(state.LazyElementRefs) { action.LazyReference }
        };
    }
}
