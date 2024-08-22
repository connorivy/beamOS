using BeamOs.Contracts.PhysicalModel.Model;
using Fluxor;

namespace BeamOs.WebApp.Client.Components.Features.ModelsPage;

public record struct UserModelsLoaded(IReadOnlyCollection<ModelResponse> ModelResponses);

public static class UserModelsLoadedReducer
{
    [ReducerMethod]
    public static ModelPageState ReduceIncrementCounterAction(
        ModelPageState state,
        UserModelsLoaded action
    ) => state with { ModelResponses = action.ModelResponses };
}
