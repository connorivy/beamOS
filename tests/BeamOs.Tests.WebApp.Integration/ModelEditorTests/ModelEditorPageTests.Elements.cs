using BeamOs.Tests.Common;
using BeamOs.Tests.WebApp.Integration.Extensions;
using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration.ModelEditorTests;

public partial class ModelEditorPageTests : ReactPageTest
{
    [Test]
    [DependsOn(nameof(NodeDialog_ShouldCreateNode))]
    [DependsOn(nameof(MaterialDialog_ShouldCreateMaterial))]
    [DependsOn(nameof(SectionProfileDialog_ShouldCreateSectionProfile))]
    public async Task Element1dDialog_ShouldCreateElement1d()
    {
        // create another node to use as the end node
        var nodesTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "nodes" }
        );
        await nodesTab.ClickAsync();

        await this.Page.FillOutNodeSelectionInfo(-4.4, -5.5, -6.6);

        // click the create button
        var createButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "create" });
        await createButton.ClickAsync();

        // click the back button
        var backButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "back" });
        await backButton.ClickAsync();

        // click the load case tab in the sidebar
        var entityTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "element1ds" }
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
            new PageGetByRoleOptions { Name = "1", Exact = true }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(0);

        // fill in value for start node id
        var startNodeIdInput = this.Page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "start node id" }
        );
        await startNodeIdInput.FillAsync("1");

        // fill in value for end node id
        var endNodeIdInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "end node id" });
        await endNodeIdInput.FillAsync("2");

        // fill in value for material id
        var materialIdInput = this.Page.GetByRole(AriaRole.Textbox, new() { Name = "material id" });
        await materialIdInput.FillAsync("1");

        // fill in value for section profile id
        var sectionProfileIdInput = this.Page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "section profile id" }
        );
        await sectionProfileIdInput.FillAsync("1");

        // fill in value for section profile rotation
        var sectionProfileRotationInput = this.Page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "section profile rotation" }
        );
        await sectionProfileRotationInput.FillAsync("30.0");

        // click the create button
        createButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "create" });
        await createButton.ClickAsync();

        // insert 1 into the load case id combobox again
        await idCombobox.FillAsync("1");

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1", Exact = true }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // refresh the page and ensure the created element1d persists
        await this.Page.ReloadAsync();

        // click the element1ds tab in the sidebar again
        entityTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "element1ds" }
        );
        await entityTab.ClickAsync();

        // insert 1 into the element1d id combobox again
        await idCombobox.FillAsync("1");
        await idCombobox.ClickAsync();

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1", Exact = true }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // select the element1d from the dropdown
        await dropdownOptions.First.ClickAsync();

        // verify that the entity inputs have the correct value
        await this.Expect(startNodeIdInput).ToHaveValueAsync("1");
        await this.Expect(endNodeIdInput).ToHaveValueAsync("2");
        await this.Expect(materialIdInput).ToHaveValueAsync("1");
        await this.Expect(sectionProfileIdInput).ToHaveValueAsync("1");
        await this.Expect(sectionProfileRotationInput).ToHaveValueAsync("30");
    }
}
