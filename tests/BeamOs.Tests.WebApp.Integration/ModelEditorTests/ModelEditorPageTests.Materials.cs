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

        await FillOutMaterialSelectionInfo(this.Page, 29000, 11500);

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
        await AssertMaterialDialogValues(this.Page, 29000, 11500);
    }

    [Test]
    [DependsOn(nameof(MaterialDialog_ShouldCreateMaterial))]
    public async Task MaterialDialog_ShouldModifyExistingMaterial()
    {
        // click the materials tab in the sidebar
        var materialsTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "materials" }
        );
        await materialsTab.ClickAsync();

        await FillOutMaterialSelectionInfo(this.Page, 29000, 11500, "1");

        // click the apply button
        var applyButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "apply" });
        await applyButton.ClickAsync();

        var materialIdCombobox = this.Page.GetByRole(
            AriaRole.Combobox,
            new PageGetByRoleOptions { Name = "id" }
        );
        await this.Expect(materialIdCombobox).ToHaveValueAsync("1");

        // test the optimistic store changes by reloading the element data without refreshing the page
        await materialIdCombobox.ClickAsync();
        var clearButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "clear" });
        await clearButton.ClickAsync();

        await materialIdCombobox.FillAsync("1");
        var dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);
        await dropdownOptions.First.ClickAsync();

        await AssertMaterialDialogValues(this.Page, 29000, 11500);

        // test the database changes by reloading the element data from the server by refreshing the page
        await this.Page.ReloadAsync();

        // click the materials tab in the sidebar again
        materialsTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "materials" }
        );
        await materialsTab.ClickAsync();

        // insert 1 into the material id combobox again
        await materialIdCombobox.FillAsync("1");
        await materialIdCombobox.ClickAsync();

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // select the material from the dropdown
        await dropdownOptions.First.ClickAsync();

        await AssertMaterialDialogValues(this.Page, 100.1, 200.2);
    }

    internal static async Task FillOutMaterialSelectionInfo(
        IPage page,
        double elasticity,
        double rigidity,
        string? materialId = null
    )
    {
        if (materialId is not null)
        {
            var materialIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "id" });
            await materialIdInput.FillAsync(materialId);

            // select the option in the dropdown
            var dropdownOption = page.GetByRole(AriaRole.Option, new() { Name = materialId });
            await dropdownOption.ClickAsync();
        }

        // fill in values for modulus of elasticity, and modulus of rigidity
        var elasticityInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "modulus of elasticity" }
        );
        await elasticityInput.FillAsync(elasticity.ToString());

        var rigidityInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "modulus of rigidity" }
        );
        await rigidityInput.FillAsync(rigidity.ToString());
    }

    internal static async Task AssertMaterialDialogValues(
        IPage page,
        double expectedElasticity,
        double expectedRigidity
    )
    {
        // fill in values for modulus of elasticity, and modulus of rigidity
        var elasticityInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "modulus of elasticity" }
        );
        await elasticityInput.ExpectToHaveApproximateValueAsync(expectedElasticity);

        var rigidityInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "modulus of rigidity" }
        );
        await rigidityInput.ExpectToHaveApproximateValueAsync(expectedRigidity);
    }
}
