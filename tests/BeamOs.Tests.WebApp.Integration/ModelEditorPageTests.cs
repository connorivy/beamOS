using BeamOs.Tests.Common;
using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration;

public class ModelEditorPageTests : ReactPageTest
{
    private static Guid? modelId;

    [Before(TUnit.Core.HookType.Test)]
    public async Task ModelEditorPage_ShouldLoadSuccessfully()
    {
        // Create a new model and navigate to its editor page
        if (modelId == null)
        {
            modelId = await this.PageContext.NavigateToNewModelPage(
                modelName: "Test Model",
                description: "This is a test model for integration testing."
            );
        }
        else
        {
            await this.Page.GotoAsync($"/models/{modelId}");
        }
    }

    [Test]
    [DependsOn(nameof(ModelEditorPage_ShouldLoadSuccessfully))]
    public async Task ModelEditorPage_CreateNodeDialog_ShouldWork()
    {
        // click the nodes tab in the sidebar
        var nodesTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "nodes" }
        );
        await nodesTab.ClickAsync();

        // insert 1 into the node id combobox
        var nodeIdCombobox = this.Page.GetByRole(
            AriaRole.Combobox,
            new PageGetByRoleOptions { Name = "node id" }
        );
        await nodeIdCombobox.ClickAsync();

        // there should not be any results in the dropdown
        var dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(0);

        // fill in values for x, y, and z textboxes
        var xInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "x" });
        await xInput.FillAsync("1.1");
        var yInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "y" });
        await yInput.FillAsync("2.2");
        var zInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "z" });
        await zInput.FillAsync("3.3");

        // click all checkboxes that contain 'translate'
        var translateCheckboxes = this.Page.GetByRole(
            AriaRole.Checkbox,
            new() { Name = "translate" }
        );
        await this.Expect(translateCheckboxes).ToHaveCountAsync(3);
        foreach (var translateCheckbox in await translateCheckboxes.AllAsync())
        {
            await translateCheckbox.ClickAsync();
        }

        // click the create button
        var createButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "create" });
        await createButton.ClickAsync();

        // insert 1 into the node id combobox again
        await nodeIdCombobox.FillAsync("1");

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // refresh the page and ensure the created node persists
        await this.Page.ReloadAsync();

        // click the nodes tab in the sidebar again
        nodesTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "nodes" }
        );
        await nodesTab.ClickAsync();

        // insert 1 into the node id combobox again
        await nodeIdCombobox.FillAsync("1");
        await nodeIdCombobox.ClickAsync();

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // select the node from the dropdown
        await dropdownOptions.First.ClickAsync();

        // verify that the x, y, and z inputs have the correct values
        await this.Expect(xInput).ToHaveValueAsync("1.1");
        await this.Expect(yInput).ToHaveValueAsync("2.2");
        await this.Expect(zInput).ToHaveValueAsync("3.3");
    }
}
