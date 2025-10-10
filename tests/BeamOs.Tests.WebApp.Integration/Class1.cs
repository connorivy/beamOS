using System.Text.RegularExpressions;
using TUnit.Playwright;

namespace BeamOs.Tests.WebApp.Integration;

public partial class Class1 : ReactPageTestWithBackend
{
    [Test]
    public async Task HomePage_ShouldLoadSuccessfully()
    {
        // Act
        await this.PageContext.Page.GotoAsync(this.GetUrl("/"));

        // Assert
        await this.Expect(this.PageContext.Page).ToHaveTitleAsync(MyRegex());
    }

    [GeneratedRegexAttribute("beamOS|react", RegexOptions.IgnoreCase, "")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
}
