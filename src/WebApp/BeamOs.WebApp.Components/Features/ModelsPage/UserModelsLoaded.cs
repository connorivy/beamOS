using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using Fluxor;

namespace BeamOs.WebApp.Components.Features.ModelsPage;

public record struct UserModelsLoaded(IReadOnlyCollection<ModelInfoResponse> ModelResponses);

public record struct ModelsDoneLoading();

public static class ModelPageStateReducers
{
    [ReducerMethod]
    public static ModelPageState ReduceIncrementCounterAction(
        ModelPageState state,
        UserModelsLoaded action
    ) => state with { UserModelResponses = action.ModelResponses };

    [ReducerMethod(typeof(ModelsDoneLoading))]
    public static ModelPageState DoneLoadingReducer(ModelPageState state) =>
        state with
        {
            IsLoading = false
        };
}
