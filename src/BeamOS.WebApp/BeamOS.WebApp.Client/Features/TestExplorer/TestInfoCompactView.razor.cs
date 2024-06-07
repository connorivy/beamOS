using BeamOS.Tests.Common.Interfaces;
using BeamOs.Tests.TestRunner;
using Microsoft.AspNetCore.Components;

namespace BeamOS.WebApp.Client.Features.TestExplorer;

public partial class TestInfoCompactView
{
    [Parameter]
    public TestInfo? TestInfo { get; set; }

    public ITestFixtureDisplayable? Displayable => this.TestInfo?.GetDisplayable();
}
