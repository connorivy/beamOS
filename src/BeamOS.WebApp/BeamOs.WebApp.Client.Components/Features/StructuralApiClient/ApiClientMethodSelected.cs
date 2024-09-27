using System.Reflection;
using Fluxor;

namespace BeamOs.WebApp.Client.Components.Features.StructuralApiClient;

public readonly record struct ApiClientMethodSelected(MethodInfo? MethodInfo);

public static class ApiClientMethodSelectedActionReducer
{
    [ReducerMethod]
    public static StructuralApiClientState ReduceApiClientMethodSelectedAction(
        StructuralApiClientState state,
        ApiClientMethodSelected action
    )
    {
        if (action.MethodInfo is null)
        {
            return state with
            {
                SelectedMethod = null,
                ParameterValues = null,
                CurrentlySelectedFieldInfo = null,
                LazyElementRefs = null,
                ElementRefs =  []
            };
        }

        var parameterType = action.MethodInfo.GetParameters().First().ParameterType;
        return state with
        {
            SelectedMethod = action.MethodInfo,
            ParameterValues = StructuralApiClientComponent.GetSettableParameterProperties(
                parameterType,
                state.ModelId,
                true
            ),
        };
    }
}
