using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Client.Components.Features.StructuralApiClient;

public readonly record struct FieldSelected(FieldInfo FieldInfo);

public record FieldInfo(string FieldName, int FieldIndex, EventCallback<object> SetValue);

public static class FieldSelectedActionReducer
{
    [ReducerMethod]
    public static StructuralApiClientState ReduceFieldSelectedAction(
        StructuralApiClientState state,
        FieldSelected action
    )
    {
        return state with { CurrentlySelectedFieldInfo = action.FieldInfo };
    }
}
