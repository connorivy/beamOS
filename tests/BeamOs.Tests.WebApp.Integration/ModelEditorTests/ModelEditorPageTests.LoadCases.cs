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

        await FillOutLoadCaseSelectionInfo(this.Page, "Test Load Case");

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

        // click the load cases tab in the sidebar again
        entityTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "load cases" }
        );
        await entityTab.ClickAsync();

        // insert 1 into the load case id combobox again
        await idCombobox.FillAsync("1");
        await idCombobox.ClickAsync();

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // select the load case from the dropdown
        await dropdownOptions.First.ClickAsync();

        // verify that the load case name input has the correct value
        await AssertLoadCaseDialogValues(this.Page, "Test Load Case");
    }

    [Test]
    [DependsOn(nameof(LoadCaseDialog_ShouldCreateLoadCase))]
    public async Task LoadCaseDialog_ShouldModifyExistingLoadCase()
    {
        // click the loadCases tab in the sidebar
        var loadCasesTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "load case" }
        );
        await loadCasesTab.ClickAsync();

        await FillOutLoadCaseSelectionInfo(this.Page, "new load case name", "1");

        // click the apply button
        var applyButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "apply" });
        await applyButton.ClickAsync();

        var loadCaseIdCombobox = this.Page.GetByRole(
            AriaRole.Combobox,
            new PageGetByRoleOptions { Name = "id" }
        );
        await this.Expect(loadCaseIdCombobox).ToHaveValueAsync("1");

        // test the optimistic store changes by reloading the element data without refreshing the page
        await loadCaseIdCombobox.ClickAsync();
        var clearButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "clear" });
        await clearButton.ClickAsync();

        await AssertLoadCaseDialogValues(this.Page, "new load case name", "1");

        // test the database changes by reloading the element data from the server by refreshing the page
        await this.Page.ReloadAsync();

        // click the loadCases tab in the sidebar again
        loadCasesTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "load case" }
        );
        await loadCasesTab.ClickAsync();

        // insert 1 into the loadCase id combobox again
        await loadCaseIdCombobox.FillAsync("1");
        await loadCaseIdCombobox.ClickAsync();

        await AssertLoadCaseDialogValues(this.Page, "new load case name", "1");
    }

    internal static async Task FillOutLoadCaseSelectionInfo(
        IPage page,
        string name,
        string? loadCaseId = null
    )
    {
        if (loadCaseId is not null)
        {
            var loadCaseIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "id" });
            await loadCaseIdInput.FillAsync(loadCaseId);

            // select the option in the dropdown
            var dropdownOption = page.GetByRole(AriaRole.Option, new() { Name = loadCaseId });
            await dropdownOption.ClickAsync();
        }

        var nameInput = page.GetByRole(AriaRole.Textbox, new() { Name = "name" });
        await nameInput.FillAsync(name);
    }

    internal static async Task AssertLoadCaseDialogValues(
        IPage page,
        string name,
        string? loadCaseId = null
    )
    {
        if (loadCaseId is not null)
        {
            var loadCaseIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "id" });
            await loadCaseIdInput.FillAsync(loadCaseId);

            // select the option in the dropdown
            var dropdownOption = page.GetByRole(AriaRole.Option, new() { Name = loadCaseId });
            await dropdownOption.ClickAsync();
        }

        var nameInput = page.GetByRole(AriaRole.Textbox, new() { Name = "name" });
        await Assertions.Expect(nameInput).ToHaveValueAsync(name);
    }
}
