using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using Fluxor;

namespace BeamOs.WebApp.Components.Features.ModelsPage;

public record struct UserModelsLoaded(IReadOnlyCollection<ModelInfoResponse> ModelResponses);

public static class UserModelsLoadedReducer
{
    [ReducerMethod]
    public static ModelPageState ReduceIncrementCounterAction(
        ModelPageState state,
        UserModelsLoaded action
    ) => state with { ModelResponses = action.ModelResponses };
}
