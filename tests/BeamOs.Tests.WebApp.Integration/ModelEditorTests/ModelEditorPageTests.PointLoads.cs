using BeamOs.Tests.Common;
using BeamOs.Tests.WebApp.Integration.Extensions;
using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration.ModelEditorTests;

public partial class ModelEditorPageTests : ReactPageTest
{
    [Test]
    [DependsOn(nameof(NodeDialog_ShouldCreateNode))]
    [DependsOn(nameof(LoadCaseDialog_ShouldCreateLoadCase))]
    public async Task PointLoadDialog_ShouldCreatePointLoad()
    {
        await Task.Delay(3000);

        var entityTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "point loads" }
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
        var loadCaseInput = this.Page.GetByRole(AriaRole.Combobox, new() { Name = "load case" });
        await loadCaseInput.FillAsync("1");

        // fill in value for node id
        var nodeIdInput = this.Page.GetByRole(AriaRole.Combobox, new() { Name = "node" });
        await nodeIdInput.FillAsync("1");

        // fill in values for force magnitude
        var fxInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "magnitude" });
        await fxInput.FillAsync("1000.0");

        // fill in values for direction
        var directionInput = this.Page.GetByRole(AriaRole.Combobox, new() { Name = "x" });
        await directionInput.FillAsync("0");

        var directionYInput = this.Page.GetByRole(AriaRole.Combobox, new() { Name = "y" });
        await directionYInput.FillAsync("0");

        var directionZInput = this.Page.GetByRole(AriaRole.Combobox, new() { Name = "z" });
        await directionZInput.FillAsync("-1");

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

        // click the point loads tab in the sidebar again
        entityTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "point loads" }
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

        // verify that the entity values have the correct value
        await this.Expect(loadCaseInput).ToHaveValueAsync("1");
        await this.Expect(nodeIdInput).ToHaveValueAsync("1");
        await this.Expect(fxInput).ToHaveValueAsync("1000");
        await this.Expect(directionInput).ToHaveValueAsync("0");
        await this.Expect(directionYInput).ToHaveValueAsync("0");
        await this.Expect(directionZInput).ToHaveValueAsync("-1");
    }
}
