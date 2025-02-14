using Fluxor;

namespace BeamOs.WebApp.Components.Layout;

[FeatureState]
public record MainLayoutState(bool IsDarkMode, bool IsDrawerOpen, bool HasContent)
{
    public MainLayoutState()
        : this(true, false, false) { }
}

public struct OpenDrawer();

public struct CloseDrawer();

public struct ToggleDarkMode();

public static class MainLayoutStateReducers
{
    [ReducerMethod(typeof(OpenDrawer))]
    public static MainLayoutState OpenDrawerReducer(MainLayoutState state)
    {
        return state with { HasContent = true };
    }

    [ReducerMethod(typeof(CloseDrawer))]
    public static MainLayoutState CloseDrawerReducer(MainLayoutState state)
    {
        return state with { HasContent = false };
    }

    [ReducerMethod(typeof(ToggleDarkMode))]
    public static MainLayoutState ToggleDarkModeReducer(MainLayoutState state)
    {
        return state with { IsDarkMode = !state.IsDarkMode };
    }
}
