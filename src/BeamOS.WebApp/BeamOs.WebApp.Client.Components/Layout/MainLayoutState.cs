using Fluxor;

namespace BeamOs.WebApp.Client.Components.Layout;

[FeatureState]
public record MainLayoutState(bool IsDrawerOpen)
{
    public MainLayoutState()
        : this(false) { }
}

public struct OpenDrawer();

public struct CloseDrawer();

public static class MainLayoutStateReducers
{
    [ReducerMethod(typeof(OpenDrawer))]
    public static MainLayoutState OpenDrawerReducer(MainLayoutState state)
    {
        return state with { IsDrawerOpen = true };
    }

    [ReducerMethod(typeof(CloseDrawer))]
    public static MainLayoutState CloseDrawerReducer(MainLayoutState state)
    {
        return state with { IsDrawerOpen = false };
    }
}
