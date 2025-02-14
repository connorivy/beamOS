using BeamOs.Tests.Common;
using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Features.TestExplorer;

public partial class RunTestButtonGroup(
    IState<TestInfoState> testInfoState,
    RunTestsInFrontEndCommandHandler runTestsInFrontEndCommandHandler
) : FluxorComponent
{
    [Parameter]
    public required string[] TestIds { get; init; }

    //protected override void OnInitialized()
    //{
    //    base.OnInitialized();
    //    singleTestState.Select(x => x.TestInfoIdToTestResultDict[this.TestId]);
    //}

    private (TestProgressStatus, TestResultStatus) GetGroupStatus()
    {
        TestProgressStatus lowestPriorityStatus = TestProgressStatus.Finished;
        TestResultStatus worstStatus = TestResultStatus.Success;

        foreach (var testId in this.TestIds)
        {
            var testState = testInfoState.Value.TestInfoIdToTestResultDict[testId];

            if (testState.FrontEndProgressStatus == TestProgressStatus.NotStarted)
            {
                return (TestProgressStatus.NotStarted, TestResultStatus.Undefined);
            }
            else if (testState.FrontEndProgressStatus == TestProgressStatus.InProgress)
            {
                lowestPriorityStatus = TestProgressStatus.InProgress;
                worstStatus = TestResultStatus.Undefined;
            }
            else
            {
                if (testState.TestResult.ResultStatus < worstStatus)
                {
                    worstStatus = testState.TestResult.ResultStatus;
                }
            }
        }

        return (lowestPriorityStatus, worstStatus);
    }

    private async Task RunAllTests()
    {
        await runTestsInFrontEndCommandHandler.ExecuteAsync(
            new RunTestsInFrontEndCommand(this.TestIds)
        );
    }
}
