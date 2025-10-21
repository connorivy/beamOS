using BeamOs.Tests.Common;
using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration;

public class ModelEditorPageTests : ReactPageTest
{
    [Test]
    public async Task CreateNode_ViaFrontend_ShouldPersist()
    {
        // Arrange - Create authenticated user and a new model
        await this.CreateAuthenticatedUser();

        await this.Page.GotoAsync(
            "/models",
            new PageGotoOptions() { WaitUntil = WaitUntilState.NetworkIdle }
        );

        // Create a new model first
        var createModelButton = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "create model" }
        );
        await createModelButton.ClickAsync();

        var dialog = this.Page.GetByRole(AriaRole.Dialog);
        await Expect(dialog).ToBeVisibleAsync();

        var modelNameInput = dialog.GetByLabel("name");
        await modelNameInput.FillAsync("Test Model for Node Creation");

        var descriptionInput = dialog.GetByLabel("description");
        await descriptionInput.FillAsync("Testing node creation");

        var lengthUnitDropdown = dialog
            .GetByRole(AriaRole.Combobox)
            .Filter(new() { HasText = "Length Unit" });
        await lengthUnitDropdown.ClickAsync();
        var footOption = this.Page.GetByRole(AriaRole.Option, new() { Name = "foot" });
        await footOption.ClickAsync();

        var forceUnitDropdown = dialog
            .GetByRole(AriaRole.Combobox)
            .Filter(new() { HasText = "Force Unit" });
        await forceUnitDropdown.ClickAsync();
        var kip = this.Page.GetByRole(AriaRole.Option, new() { Name = "kilopoundforce" });
        await kip.ClickAsync();

        var submitButton = dialog.GetByRole(AriaRole.Button, new() { Name = "create" });
        await submitButton.ClickAsync();

        // Wait for navigation to model editor page
        await this.Page.WaitForURLAsync(
            new System.Text.RegularExpressions.Regex(
                "^(http|https)://localhost:\\d+/models/[0-9a-fA-F-]{36}$"
            ),
            new() { Timeout = 3000 }
        );

        // Act - Click 'Nodes' button in sidebar to open the node editor
        var nodesButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "Nodes" });
        await nodesButton.ClickAsync();

        // Wait for node editor panel to appear and the back button to be visible
        var backButton = this.Page.GetByRole(AriaRole.Button).Filter(new() { HasText = "Back" });
        await Expect(backButton).ToBeVisibleAsync();

        // Wait a bit for the form to fully load
        await this.Page.WaitForTimeoutAsync(1000);

        // Find and fill the X coordinate input - these are in the LocationPoint expansion panel
        // MudBlazor number inputs are rendered as regular HTML inputs
        var inputs = await this.Page.Locator("input[type='number']").AllAsync();

        // The first three numeric inputs should be X, Y, Z based on the NodeObjectEditor.razor
        if (inputs.Count >= 3)
        {
            await inputs[0].FillAsync("10"); // X
            await inputs[1].FillAsync("20"); // Y
            await inputs[2].FillAsync("30"); // Z
        }

        // Click 'Apply' button to create the node
        // The button is rendered as "Apply" in the NodeObjectEditor
        var applyButton = this.Page.GetByRole(AriaRole.Button).Filter(new() { HasText = "Apply" });
        await applyButton.ClickAsync();

        // Wait for the node to be created
        await this.Page.WaitForTimeoutAsync(1000);

        // Assert - Check that the new node was added to the model
        // We'll verify by checking if we can see the node in the autocomplete/dropdown
        var nodeIdAutocomplete = this.Page.GetByLabel("Node Id");
        await nodeIdAutocomplete.ClickAsync();

        // The new node should appear in the list (ID will be 1 for first node)
        var nodeOption = this.Page.GetByText("1");
        await Expect(nodeOption).ToBeVisibleAsync();

        // Refresh the page to verify persistence
        await this.Page.ReloadAsync(
            new PageReloadOptions() { WaitUntil = WaitUntilState.NetworkIdle }
        );

        // Wait for editor to reload
        await this.Page.WaitForTimeoutAsync(1000);

        // Click Nodes button again after refresh
        var nodesButtonAfterRefresh = this.Page.GetByRole(
            AriaRole.Button,
            new() { Name = "Nodes" }
        );
        await nodesButtonAfterRefresh.ClickAsync();

        await this.Page.WaitForTimeoutAsync(500);

        // Check that the node still appears after refresh
        var nodeIdAutocompleteAfterRefresh = this.Page.GetByLabel("Node Id");
        await nodeIdAutocompleteAfterRefresh.ClickAsync();

        var nodeOptionAfterRefresh = this.Page.GetByText("1");
        await Expect(nodeOptionAfterRefresh).ToBeVisibleAsync();
    }

    private Task CreateAuthenticatedUser() =>
        AssemblySetup.CreateAuthenticatedUser(this.PageContext);
}
