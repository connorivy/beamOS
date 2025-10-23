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

    [Test]
    public async Task ModelEditorPage_MaterialDialog_ShouldWork()
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

    [Test]
    public async Task ModelEditorPage_SectionProfileDialog_ShouldWork()
    {
        // click the nodes tab in the sidebar
        var sectionTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "sections" }
        );
        await sectionTab.ClickAsync();

        // insert 1 into the material id combobox
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

        // fill in name
        var nameInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "name" });
        await nameInput.FillAsync("Test Section Profile");

        // fill in area
        var areaInput = this.Page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "area", Exact = true }
        );
        await areaInput.FillAsync("10.5");

        // fill in strong axis moment of inertia
        var strongAxisInput = this.Page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "strong axis moment of inertia" }
        );
        await strongAxisInput.FillAsync("200.0");

        // fill in weak axis moment of inertia
        var weakAxisInput = this.Page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "weak axis moment of inertia" }
        );
        await weakAxisInput.FillAsync("150.0");

        // fill in polar moment of inertia
        var polarInput = this.Page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "polar moment of inertia" }
        );
        await polarInput.FillAsync("300.0");

        // fill in strong axis plastic section modulus
        var strongAxisPlasticInput = this.Page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "strong axis plastic section modulus" }
        );
        await strongAxisPlasticInput.FillAsync("25.0");

        // fill in weak axis plastic section modulus
        var weakAxisPlasticInput = this.Page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "weak axis plastic section modulus" }
        );
        await weakAxisPlasticInput.FillAsync("20.0");

        // fill in weak axis shear area
        var shearAreaInput = this.Page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "weak axis shear area" }
        );
        await shearAreaInput.FillAsync("8.0");

        // fill in strong axis shear area
        var strongShearAreaInput = this.Page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "strong axis shear area" }
        );
        await strongShearAreaInput.FillAsync("9.0");

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

        // refresh the page and ensure the enitity persists
        await this.Page.ReloadAsync();

        // click the tab in the sidebar again
        sectionTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "sections" }
        );
        await sectionTab.ClickAsync();

        // insert 1 into the section id combobox again
        await idCombobox.FillAsync("1");
        await idCombobox.ClickAsync();

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // select the section from the dropdown
        await dropdownOptions.First.ClickAsync();

        // verify that the modulus of elasticity and modulus of rigidity inputs have the correct values
        await this.Expect(nameInput).ToHaveValueAsync("Test Section Profile");
        await this.Expect(areaInput).ToHaveValueAsync("10.5");
        await this.Expect(strongAxisInput).ToHaveValueAsync("200.0");
        await this.Expect(weakAxisInput).ToHaveValueAsync("150.0");
        await this.Expect(polarInput).ToHaveValueAsync("300.0");
        await this.Expect(strongAxisPlasticInput).ToHaveValueAsync("25.0");
        await this.Expect(weakAxisPlasticInput).ToHaveValueAsync("20.0");
        await this.Expect(shearAreaInput).ToHaveValueAsync("8.0");
        await this.Expect(strongShearAreaInput).ToHaveValueAsync("9.0");
    }

    [Test]
    public async Task ModelEditorPage_LoadCaseDialog_ShouldWork()
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

    [Test]
    [DependsOn(nameof(ModelEditorPage_CreateNodeDialog_ShouldWork))]
    public async Task ModelEditorPage_MomentLoadDialog_ShouldWork()
    {
        var entityTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "moment loads" }
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

        // fill in value for load case id
        var loadCaseInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "load case" });
        await loadCaseInput.FillAsync("1");

        // fill in value for node id
        var nodeIdInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "node id" });
        await nodeIdInput.FillAsync("1");

        // fill in value for magnitude
        var magnitudeInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "magnitude" });
        await magnitudeInput.FillAsync("500.0");

        // fill in value for direction
        var directionInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "x" });
        await directionInput.FillAsync("1.0");

        var directionYInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "y" });
        await directionYInput.FillAsync("0.0");

        var directionZInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "z" });
        await directionZInput.FillAsync("0.0");

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
        await this.Expect(loadCaseInput).ToHaveValueAsync("1");
        await this.Expect(nodeIdInput).ToHaveValueAsync("1");
        await this.Expect(magnitudeInput).ToHaveValueAsync("500.0");
        await this.Expect(directionInput).ToHaveValueAsync("1.0");
        await this.Expect(directionYInput).ToHaveValueAsync("0.0");
        await this.Expect(directionZInput).ToHaveValueAsync("0.0");
    }
}
