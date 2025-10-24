using BeamOs.Tests.Common;
using BeamOs.Tests.WebApp.Integration.Extensions;
using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration.ModelEditorTests;

public partial class ModelEditorPageTests : ReactPageTest
{
    [Test]
    [DependsOn(nameof(NodeDialog_ShouldCreateNode))]
    [DependsOn(nameof(LoadCaseDialog_ShouldCreateLoadCase))]
    public async Task MomentLoadDialog_ShouldCreateMomentLoad()
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
        var loadCaseInput = this.Page.GetByRole(AriaRole.Combobox, new() { Name = "load case" });
        await loadCaseInput.FillAsync("1");

        // fill in value for node id
        var nodeIdInput = this.Page.GetByRole(AriaRole.Combobox, new() { Name = "node" });
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

        // refresh the page and ensure the created moment load persists
        await this.Page.ReloadAsync();

        // click the moment loads tab in the sidebar again
        entityTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "moment loads" }
        );
        await entityTab.ClickAsync();

        // insert 1 into the moment load id combobox again
        await idCombobox.FillAsync("1");
        await idCombobox.ClickAsync();

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // select the moment load from the dropdown
        await dropdownOptions.First.ClickAsync();

        // verify that the moment load inputs have the correct values
        await this.Expect(loadCaseInput).ToHaveValueAsync("1");
        await this.Expect(nodeIdInput).ToHaveValueAsync("1");
        await magnitudeInput.ExpectToHaveApproximateValueAsync(500);
        await directionInput.ExpectToHaveApproximateValueAsync(1);
        await directionYInput.ExpectToHaveApproximateValueAsync(0);
        await directionZInput.ExpectToHaveApproximateValueAsync(0);
    }
}
