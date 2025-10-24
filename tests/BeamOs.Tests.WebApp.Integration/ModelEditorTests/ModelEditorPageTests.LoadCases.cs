using BeamOs.Tests.Common;
using BeamOs.Tests.WebApp.Integration.Extensions;
using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration.ModelEditorTests;

public partial class ModelEditorPageTests : ReactPageTest
{
    [Test]
    public async Task LoadCaseDialog_ShouldCreateLoadCase()
    {
        // click the load case tab in the sidebar
        var entityTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "load cases" }
        );
        await entityTab.ClickAsync();

        // insert 1 into the load case id combobox
        var idCombobox = this.Page.GetByRole(
            AriaRole.Combobox,
            new PageGetByRoleOptions { Name = "id" }
        );
        await idCombobox.ClickAsync();
        // there should not be any results in the dropdown
        var dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(0);

        // fill in value for load case name
        var nameInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "name" });
        await nameInput.FillAsync("Test Load Case");

        // click the create button
        var createButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "create" });
        await createButton.ClickAsync();

        // insert 1 into the load case id combobox again
        await idCombobox.FillAsync("1");

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // refresh the page and ensure the created node persists
        await this.Page.ReloadAsync();

        // click the nodes tab in the sidebar again
        entityTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "load cases" }
        );
        await entityTab.ClickAsync();

        // insert 1 into the node id combobox again
        await idCombobox.FillAsync("1");
        await idCombobox.ClickAsync();

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // select the node from the dropdown
        await dropdownOptions.First.ClickAsync();

        // verify that the load case name input has the correct value
        await this.Expect(nameInput).ToHaveValueAsync("Test Load Case");
    }
}
