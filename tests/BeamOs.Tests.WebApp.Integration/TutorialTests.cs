using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration;

public class TutorialTests : BlazorPageTest
{
    [Test]
    public async Task WalkthroughTutorial_ShouldWorkCorrectly()
    {
        // Act
        await this.PageContext.Page.GotoAsync(
            "/models",
            new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle }
        );

        // Find the card with the heading "Tutorial" and click it
        var tutorialCard = this
            .Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Tutorial" })
            .First;
        await tutorialCard.ClickAsync();

        // Assert - Verify that the URL has changed to the tutorial page
        await this.Expect(this.Page).ToHaveURLAsync("/tutorial");

        // Verify that a pop-up appears with the tutorial content
        var tutorialPopup = this.Page.GetByRole(
            AriaRole.Dialog,
            new PageGetByRoleOptions { Name = "tutorial" }
        );
        await this.Expect(tutorialPopup).ToBeVisibleAsync();

        // Click through the tutorial steps
        var nextButton = tutorialPopup.GetByRole(
            AriaRole.Button,
            new LocatorGetByRoleOptions { Name = "next" }
        );
        for (int i = 0; i < 5; i++)
        {
            await nextButton.ClickAsync();
            await this.Page.WaitForTimeoutAsync(500); // Wait for half a second between clicks
        }

        // Close the tutorial pop-up
        var closeButton = tutorialPopup.GetByRole(
            AriaRole.Button,
            new LocatorGetByRoleOptions { Name = "close" }
        );
        await closeButton.ClickAsync();
        await this.Expect(tutorialPopup).Not.ToBeVisibleAsync();
    }
}
