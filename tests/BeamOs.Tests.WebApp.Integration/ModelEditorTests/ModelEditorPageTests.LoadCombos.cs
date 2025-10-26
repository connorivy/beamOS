using BeamOs.Tests.Common;
using BeamOs.Tests.WebApp.Integration.Extensions;
using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration.ModelEditorTests;

public partial class ModelEditorPageTests : ReactPageTest
{
    [Test]
    [DependsOn(nameof(LoadCaseDialog_ShouldCreateLoadCase))]
    public async Task LoadComboDialog_ShouldCreateLoadCombo()
    {
        // click the load combination tab in the sidebar
        var entityTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "load case" }
        );
        await entityTab.ClickAsync();

        await FillOutLoadCaseSelectionInfo(this.Page, "load case 2");

        // click the create button
        var createButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "create" });
        await createButton.ClickAsync();

        // click the back button to return to the model editor main page
        var backButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "back" });
        await backButton.ClickAsync();

        // click the load combination tab in the sidebar
        entityTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "load combinations" }
        );
        await entityTab.ClickAsync();

        // insert 1 into the load combination id combobox
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

        await FillOutLoadComboSelectionInfo(this.Page, [("1", 1.5), ("2", 0.75)]);

        // click the create button
        createButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "create" });
        await createButton.ClickAsync();

        // insert 1 into the load case id combobox again
        await idCombobox.FillAsync("1");

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // refresh the page and ensure the created load combination persists
        await this.Page.ReloadAsync();

        // click the load combinations tab in the sidebar again
        entityTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "load combinations" }
        );
        await entityTab.ClickAsync();

        // insert 1 into the load combination id combobox again
        await idCombobox.FillAsync("1");
        await idCombobox.ClickAsync();

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // select the load combination from the dropdown
        await dropdownOptions.First.ClickAsync();

        // verify that the load case factors have the correct values
        // Note: Since API uses dictionary structure, duplicate load case IDs will be merged, keeping only the last factor
        await AssertLoadComboDialogValues(this.Page, [("1", 1.5), ("2", 0.75)]);
    }

    [Test]
    [DependsOn(nameof(LoadComboDialog_ShouldCreateLoadCombo))]
    public async Task LoadComboDialog_ShouldModifyExistingLoadCombo()
    {
        // click the loadCombos tab in the sidebar
        var loadCombosTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "load comb" }
        );
        await loadCombosTab.ClickAsync();

        await FillOutLoadComboSelectionInfo(this.Page, [("1", 2.0), ("2", 0.5)], "1");

        // click the apply button
        var applyButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "apply" });
        await applyButton.ClickAsync();

        var loadComboIdCombobox = this.Page.GetByRole(
            AriaRole.Combobox,
            new PageGetByRoleOptions { Name = "id" }
        );
        await this.Expect(loadComboIdCombobox).ToHaveValueAsync("1");

        // test the optimistic store changes by reloading the element data without refreshing the page
        await loadComboIdCombobox.ClickAsync();
        var clearButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "clear" });
        await clearButton.ClickAsync();

        await AssertLoadComboDialogValues(this.Page, [("1", 2.0), ("2", 0.5)], "1");

        // test the database changes by reloading the element data from the server by refreshing the page
        await this.Page.ReloadAsync();

        // click the loadCombos tab in the sidebar again
        loadCombosTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "load comb" }
        );
        await loadCombosTab.ClickAsync();

        // insert 1 into the loadCombo id combobox again
        await loadComboIdCombobox.FillAsync("1");
        await loadComboIdCombobox.ClickAsync();

        await AssertLoadComboDialogValues(this.Page, [("1", 2.0), ("2", 0.5)], "1");
    }

    internal static async Task FillOutLoadComboSelectionInfo(
        IPage page,
        IEnumerable<(string LoadCaseId, double Factor)> loadCases,
        string? loadComboId = null
    )
    {
        if (loadComboId is not null)
        {
            var loadComboIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "id" });
            await loadComboIdInput.FillAsync(loadComboId);

            // select the option in the dropdown
            var dropdownOption = page.GetByRole(AriaRole.Option, new() { Name = loadComboId });
            await dropdownOption.ClickAsync();
        }

        int index = -1;
        foreach (var (loadCaseId, factor) in loadCases)
        {
            index++;
            // find all load case id input dropdowns
            var loadCaseInput = page.GetByRole(AriaRole.Combobox, new() { Name = "load case" });
            await loadCaseInput.Nth(index).FillAsync(loadCaseId);
            // find all factor inputs
            var factorInputs = page.GetByRole(AriaRole.Textbox, new() { Name = "factor" });
            await factorInputs.Nth(index).FillAsync(factor.ToString());
            // a new load combination should now be added to the list
        }
    }

    internal static async Task AssertLoadComboDialogValues(
        IPage page,
        IEnumerable<(string LoadCaseId, double Factor)> loadCases,
        string? loadComboId = null
    )
    {
        if (loadComboId is not null)
        {
            var loadComboIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "id" });
            await loadComboIdInput.FillAsync(loadComboId);

            // select the option in the dropdown
            var dropdownOption = page.GetByRole(AriaRole.Option, new() { Name = loadComboId });
            await dropdownOption.ClickAsync();
        }

        int index = -1;
        foreach (var (loadCaseId, factor) in loadCases)
        {
            index++;
            // find all load combo id input dropdowns
            var loadCaseInput = page.GetByRole(AriaRole.Combobox, new() { Name = "load case" });
            await Assertions.Expect(loadCaseInput.Nth(index)).ToHaveValueAsync(loadCaseId);
            // find all factor inputs
            var factorInputs = page.GetByRole(AriaRole.Textbox, new() { Name = "factor" });
            await Assertions.Expect(factorInputs.Nth(index)).ToHaveValueAsync(factor.ToString());
        }
    }
}
