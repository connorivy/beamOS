using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using BeamOs.Tests.Common;
using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration;

public class ModelsPageTests : ReactPageTest
{
    [Test]
    public async Task CreateModelDialog_ShouldWork()
    {
        await this.CreateAuthenticatedUser();

        await this.Page.GotoAsync(
            "/models",
            new PageGotoOptions() { WaitUntil = WaitUntilState.NetworkIdle }
        );

        // find the create model button and click it
        var createModelButton = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "create model" }
        );
        await createModelButton.ClickAsync();

        // the 'create model' dialog should be visible
        var dialog = this.Page.GetByRole(AriaRole.Dialog);
        await Expect(dialog).ToBeVisibleAsync();

        // fill the model name input
        var modelNameInput = dialog.GetByLabel("name");
        await modelNameInput.FillAsync("My Test Model");

        // fill the description input
        var descriptionInput = dialog.GetByLabel("description");
        await descriptionInput.FillAsync("This is a test model");

        // select the length unit from the dropdown
        var lengthUnitDropdown = dialog
            .GetByRole(AriaRole.Combobox)
            .Filter(new() { HasText = "Length Unit" });
        await lengthUnitDropdown.ClickAsync();
        var footOption = this.Page.GetByRole(AriaRole.Option, new() { Name = "foot" });
        await footOption.ClickAsync();

        // select the kilopoundforce unit from the forceunit dropdown
        var forceUnitDropdown = dialog
            .GetByRole(AriaRole.Combobox)
            .Filter(new() { HasText = "Force Unit" });
        await forceUnitDropdown.ClickAsync();
        var kip = this.Page.GetByRole(AriaRole.Option, new() { Name = "kilopoundforce" });
        await kip.ClickAsync();

        // click the submit button and wait for the POST request to create the model
        var submitButton = dialog.GetByRole(AriaRole.Button, new() { Name = "create" });
        var modelResponseTask = this.Page.WaitForResponseAsync(r =>
            r.Url.Contains("/models") && r.Request.Method == "POST", 
            new() { Timeout = System.Diagnostics.Debugger.IsAttached ? 0 : 15_000}
        );
        await submitButton.ClickAsync();
        
        // Wait for the POST request to complete
        await modelResponseTask;

        // wait for the page url to look something like {http}://localhost:{port}/models/{guid}
        // Use Expect().ToHaveURLAsync() instead of WaitForURLAsync() because this is a SPA navigation
        // that doesn't trigger a full page load event
        await Expect(this.Page).ToHaveURLAsync(
            new Regex("^(http|https)://localhost:\\d+/models/[0-9a-fA-F-]{36}$"),
            new() { Timeout = System.Diagnostics.Debugger.IsAttached ? 0 : 15_000 }
        );

        // go back to the models page
        await this.Page.GotoAsync(
            "/models",
            new PageGotoOptions() { WaitUntil = WaitUntilState.NetworkIdle }
        );
        // 'My Test Model' should be visible on the page
        var modelCard = this.Page.GetByRole(
            AriaRole.Heading,
            new PageGetByRoleOptions { Name = "My Test Model", Exact = true }
        );
        await Expect(modelCard).ToBeVisibleAsync();
    }

    private Task CreateAuthenticatedUser() =>
        AssemblySetup.CreateAuthenticatedUser(this.PageContext);
}
