using BeamOs.Tests.Common;
using BeamOs.Tests.WebApp.Integration.Extensions;
using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration.ModelEditorTests;

public partial class ModelEditorPageTests : ReactPageTest
{
    [Test]
    public async Task NodeDialog_ShouldCreateNode()
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

    [Test]
    [DependsOn(nameof(NodeDialog_ShouldCreateNode))]
    public async Task NodeDialog_ShouldModifyExistingNode()
    {
        // click the nodes tab in the sidebar
        var nodesTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "nodes" }
        );
        await nodesTab.ClickAsync();

        await this.Page.FillOutNodeSelectionInfo(
            100.1,
            200.2,
            300.3,
            "1",
            true,
            true,
            true,
            true,
            true,
            true
        );

        // click the apply button
        var applyButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "apply" });
        await applyButton.ClickAsync();

        var nodeIdCombobox = this.Page.GetByRole(
            AriaRole.Combobox,
            new PageGetByRoleOptions { Name = "id" }
        );
        await this.Expect(nodeIdCombobox).ToHaveValueAsync("1");

        // test the optimistic store changes by reloading the element data without refreshing the page
        await nodeIdCombobox.ClickAsync();
        var clearButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "clear" });

        await nodeIdCombobox.FillAsync("1");
        var dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);
        await dropdownOptions.First.ClickAsync();

        // verify that the x, y, and z inputs have the correct values
        var xInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "x" });
        var yInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "y" });
        var zInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "z" });
        await xInput.ExpectToHaveApproximateValueAsync(100.1);
        await yInput.ExpectToHaveApproximateValueAsync(200.2);
        await zInput.ExpectToHaveApproximateValueAsync(300.3);

        // verify that all translate and rotate checkboxes are checked
        var translateCheckboxes = this.Page.GetByRole(AriaRole.Checkbox, new() { Name = "along" });
        await this.Expect(translateCheckboxes).ToHaveCountAsync(3);
        foreach (var translateCheckbox in await translateCheckboxes.AllAsync())
        {
            await this.Expect(translateCheckbox).ToBeCheckedAsync();
        }

        var rotateCheckboxes = this.Page.GetByRole(AriaRole.Checkbox, new() { Name = "about" });
        await this.Expect(rotateCheckboxes).ToHaveCountAsync(3);
        foreach (var rotateCheckbox in await rotateCheckboxes.AllAsync())
        {
            await this.Expect(rotateCheckbox).ToBeCheckedAsync();
        }

        // test the database changes by reloading the element data from the server by refreshing the page
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
        xInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "x" });
        yInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "y" });
        zInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "z" });
        await xInput.ExpectToHaveApproximateValueAsync(100.1);
        await yInput.ExpectToHaveApproximateValueAsync(200.2);
        await zInput.ExpectToHaveApproximateValueAsync(300.3);

        // verify that all translate and rotate checkboxes are checked
        translateCheckboxes = this.Page.GetByRole(AriaRole.Checkbox, new() { Name = "along" });
        await this.Expect(translateCheckboxes).ToHaveCountAsync(3);
        foreach (var translateCheckbox in await translateCheckboxes.AllAsync())
        {
            await this.Expect(translateCheckbox).ToBeCheckedAsync();
        }

        rotateCheckboxes = this.Page.GetByRole(AriaRole.Checkbox, new() { Name = "about" });
        await this.Expect(rotateCheckboxes).ToHaveCountAsync(3);
        foreach (var rotateCheckbox in await rotateCheckboxes.AllAsync())
        {
            await this.Expect(rotateCheckbox).ToBeCheckedAsync();
        }
    }
}
