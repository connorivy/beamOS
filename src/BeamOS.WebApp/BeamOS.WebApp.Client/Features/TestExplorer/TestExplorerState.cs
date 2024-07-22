using BeamOS.Tests.Common.Interfaces;
using BeamOs.Tests.TestRunner;
using Fluxor;

namespace BeamOS.WebApp.Client.Pages;

[FeatureState]
public record TestExplorerState(TestInfo? SelectedTestInfo, string? SelectedTrait)
{
    private TestExplorerState()
        : this(null, null) { }
}

public readonly record struct ChangeSelectedTestInfoAction(TestInfo? SelectedTestInfo);

public readonly record struct ChangeSelectedTraitAction(string SelectedTrait);

public static class TestExplorerStateFluxor
{
    [ReducerMethod]
    public static TestExplorerState ChangeSelectedTestInfoReducer(
        TestExplorerState state,
        ChangeSelectedTestInfoAction changeSelectedTestInfoAction
    )
    {
        return state with { SelectedTestInfo = changeSelectedTestInfoAction.SelectedTestInfo };
    }

    [ReducerMethod]
    public static TestExplorerState ChangeSelectedTraitReducer(
        TestExplorerState state,
        ChangeSelectedTraitAction changeSelectedTraitAction
    )
    {
        return state with { SelectedTrait = changeSelectedTraitAction.SelectedTrait };
    }

    //[ReducerMethod]
    //public static TestExplorerState ChangeTestFixtureDisplayableReducer(
    //    TestExplorerState state,
    //    ChangeTestFixtureDisplayableAction changeTestFixtureDisplayableAction
    //)
    //{
    //    return state with
    //    {
    //        TestFixtureDisplayable = changeTestFixtureDisplayableAction.TestFixtureDisplayable
    //    };
    //}
}
