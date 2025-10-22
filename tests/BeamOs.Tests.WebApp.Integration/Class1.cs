using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration;

public partial class Class1 : ReactPageTest
{
    [Test]
    public async Task HomePage_ShouldLoadSuccessfully()
    {
        // Act
        await this.PageContext.Page.GotoAsync(
            "/",
            new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle }
        );

        // Assert
        await this.Expect(this.PageContext.Page).ToHaveTitleAsync("beamOS");
    }
}
