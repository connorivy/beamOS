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

        // find all load case id input dropdowns
        var loadCaseInputs = this.Page.GetByRole(AriaRole.Combobox, new() { Name = "load case" });
        await this.Expect(loadCaseInputs).ToHaveCountAsync(1);
        await loadCaseInputs.First.FillAsync("1");

        // find all factor inputs
        var factorInputs = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "factor" });
        await this.Expect(factorInputs).ToHaveCountAsync(1);
        await factorInputs.First.FillAsync("1.5");

        // a new load combination should now be added to the list
        loadCaseInputs = this.Page.GetByRole(AriaRole.Combobox, new() { Name = "load case" });
        await this.Expect(loadCaseInputs).ToHaveCountAsync(2);
        await loadCaseInputs.Nth(1).FillAsync("1");

        factorInputs = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "factor" });
        await this.Expect(factorInputs).ToHaveCountAsync(2);
        await factorInputs.Nth(1).FillAsync("0.75");

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
        loadCaseInputs = this.Page.GetByRole(AriaRole.Combobox, new() { Name = "load case" });
        await this.Expect(loadCaseInputs).ToHaveCountAsync(1);
        await this.Expect(loadCaseInputs.First).ToHaveValueAsync("1");
        factorInputs = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "factor" });
        await this.Expect(factorInputs).ToHaveCountAsync(1);
        await this.Expect(factorInputs.First).ToHaveValueAsync("0.75");
    }
}
