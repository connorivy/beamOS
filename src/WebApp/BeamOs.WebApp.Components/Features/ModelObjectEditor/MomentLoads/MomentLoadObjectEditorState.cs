using Fluxor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.MomentLoads;

[FeatureState]
public record MomentLoadObjectEditorState(bool IsLoading)
{
    public MomentLoadObjectEditorState()
        : this(false) { }
}

public record struct MomentLoadObjectEditorIsLoading(bool IsLoading);

public static class MomentLoadObjectEditorStateReducers
{
    [ReducerMethod]
    public static MomentLoadObjectEditorState Reducer(
        MomentLoadObjectEditorState state,
        MomentLoadObjectEditorIsLoading action
    ) => state with { IsLoading = action.IsLoading };
}

