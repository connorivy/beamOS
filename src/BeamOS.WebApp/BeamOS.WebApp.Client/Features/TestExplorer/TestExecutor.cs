using System;
using BeamOs.Tests.TestRunner;
using Fluxor;

namespace BeamOS.WebApp.Client.Features.TestExplorer;

public class TestExecutor
{
    private readonly TestInfoProvider testInfoProvider;
    private readonly IState<TestInfoState> testInfoState;

    public TestExecutor(TestInfoProvider testInfoProvider, IState<TestInfoState> testInfoState)
    {
        this.testInfoProvider = testInfoProvider;
        this.testInfoState = testInfoState;
    }

    [EffectMethod]
    public async Task HandleExecuteTestAction(
        ExecutionTestActionById action,
        IDispatcher dispatcher
    ) => await this.HandleExecuteTestAction(action.TestId, dispatcher);

    [EffectMethod]
    public async Task HandleExecuteTestAction(ExecutionTestAction action, IDispatcher dispatcher) =>
        await this.HandleExecuteTestAction(action.TestInfo.Id, dispatcher);

    private async Task HandleExecuteTestAction(string testId, IDispatcher dispatcher)
    {
        TestInfo testInfo = this.testInfoProvider.TestInfos[testId];

        // todo : get and add to cache
        TestResult result = await testInfo.RunTest();
        dispatcher.Dispatch(new ExecutionTestActionResult(testId, result));
    }
}
