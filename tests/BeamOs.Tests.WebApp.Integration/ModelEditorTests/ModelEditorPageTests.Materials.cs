using BeamOs.Tests.Common;
using BeamOs.Tests.WebApp.Integration.Extensions;
using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration.ModelEditorTests;

public partial class ModelEditorPageTests : ReactPageTest
{
    [Test]
    public async Task MaterialDialog_ShouldCreateMaterial()
    {
        // click the nodes tab in the sidebar
        var nodesTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "materials" }
        );
        await nodesTab.ClickAsync();

        // insert 1 into the material id combobox
        var idCombobox = this.Page.GetByRole(
            AriaRole.Combobox,
            new PageGetByRoleOptions { Name = "material id" }
        );
        await idCombobox.ClickAsync();
        // there should not be any results in the dropdown
        var dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(0);

        // fill in values for modulus of elasticity, and modulus of rigidity
        var elasticityInput = this.Page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "modulus of elasticity" }
        );
        await elasticityInput.FillAsync("29000");
        var rigidityInput = this.Page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "modulus of rigidity" }
        );
        await rigidityInput.FillAsync("11500");

        // click the create button
        var createButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "create" });
        await createButton.ClickAsync();

        // insert 1 into the material id combobox again
        await idCombobox.FillAsync("1");

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // refresh the page and ensure the created material persists
        await this.Page.ReloadAsync();

        // click the materials tab in the sidebar again
        nodesTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "materials" }
        );
        await nodesTab.ClickAsync();

        // insert 1 into the material id combobox again
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

        // verify that the modulus of elasticity and modulus of rigidity inputs have the correct values
        await this.Expect(elasticityInput).ToHaveValueAsync("29000");
        await this.Expect(rigidityInput).ToHaveValueAsync("11500");
    }
}
