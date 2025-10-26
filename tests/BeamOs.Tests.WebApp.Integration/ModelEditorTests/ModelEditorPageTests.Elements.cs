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

        await FillOutElement1dSelectionInfo(this.Page, 1, 2, 1, 1, 30.0);

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

        // // now there should be one result in the dropdown
        // dropdownOptions = this.Page.GetByRole(
        //     AriaRole.Option,
        //     new PageGetByRoleOptions { Name = "1", Exact = true }
        // );
        // await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // // select the element1d from the dropdown
        // await dropdownOptions.First.ClickAsync();

        // verify that the entity inputs have the correct value
        await AssertElement1dDialogValues(this.Page, 1, 2, 1, 1, 30.0, "1");
    }

    [Test]
    [DependsOn(nameof(Element1dDialog_ShouldCreateElement1d))]
    public async Task Element1dDialog_ShouldModifyExistingElement1d()
    {
        // click the element1ds tab in the sidebar
        var element1dsTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "element1ds" }
        );
        await element1dsTab.ClickAsync();

        await FillOutElement1dSelectionInfo(this.Page, 1, 2, 1, 1, 45.0, "1");

        // click the apply button
        var applyButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "apply" });
        await applyButton.ClickAsync();

        var element1dIdCombobox = this.Page.GetByRole(
            AriaRole.Combobox,
            new PageGetByRoleOptions { Name = "id" }
        );
        await this.Expect(element1dIdCombobox).ToHaveValueAsync("1");

        // test the optimistic store changes by reloading the element data without refreshing the page
        await element1dIdCombobox.ClickAsync();
        var clearButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "clear" });
        await clearButton.ClickAsync();

        await AssertElement1dDialogValues(this.Page, 1, 2, 1, 1, 45.0, "1");

        // test the database changes by reloading the element data from the server by refreshing the page
        await this.Page.ReloadAsync();

        // click the element1ds tab in the sidebar again
        element1dsTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "element1ds" }
        );
        await element1dsTab.ClickAsync();

        await AssertElement1dDialogValues(this.Page, 1, 2, 1, 1, 45.0, "1");
    }

    internal static async Task FillOutElement1dSelectionInfo(
        IPage page,
        int startNodeId,
        int endNodeId,
        int materialId,
        int sectionProfileId,
        double sectionProfileRotation,
        string? element1dId = null
    )
    {
        if (element1dId is not null)
        {
            var element1dIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "id" });
            await element1dIdInput.FillAsync(element1dId);

            // select the option in the dropdown
            var dropdownOption = page.GetByRole(
                AriaRole.Option,
                new() { Name = element1dId, Exact = true }
            );
            await dropdownOption.ClickAsync();
        }

        var startNodeIdInput = page.GetByRole(AriaRole.Textbox, new() { Name = "start node id" });
        await startNodeIdInput.FillAsync(startNodeId.ToString());

        var endNodeIdInput = page.GetByRole(AriaRole.Textbox, new() { Name = "end node id" });
        await endNodeIdInput.FillAsync(endNodeId.ToString());

        var materialIdInput = page.GetByRole(AriaRole.Textbox, new() { Name = "material id" });
        await materialIdInput.FillAsync(materialId.ToString());

        var sectionProfileIdInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "section profile id" }
        );
        await sectionProfileIdInput.FillAsync(sectionProfileId.ToString());

        var sectionProfileRotationInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "section profile rotation" }
        );
        await sectionProfileRotationInput.FillAsync(sectionProfileRotation.ToString());
    }

    internal static async Task AssertElement1dDialogValues(
        IPage page,
        int startNodeId,
        int endNodeId,
        int materialId,
        int sectionProfileId,
        double sectionProfileRotation,
        string? element1dId = null
    )
    {
        if (element1dId is not null)
        {
            var element1dIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "id" });
            await element1dIdInput.FillAsync(element1dId);

            // select the option in the dropdown
            var dropdownOption = page.GetByRole(
                AriaRole.Option,
                new() { Name = element1dId, Exact = true }
            );
            await dropdownOption.ClickAsync();
        }

        var startNodeIdInput = page.GetByRole(AriaRole.Textbox, new() { Name = "start node id" });
        await Assertions.Expect(startNodeIdInput).ToHaveValueAsync(startNodeId.ToString());

        var endNodeIdInput = page.GetByRole(AriaRole.Textbox, new() { Name = "end node id" });
        await Assertions.Expect(endNodeIdInput).ToHaveValueAsync(endNodeId.ToString());

        var materialIdInput = page.GetByRole(AriaRole.Textbox, new() { Name = "material id" });
        await Assertions.Expect(materialIdInput).ToHaveValueAsync(materialId.ToString());

        var sectionProfileIdInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "section profile id" }
        );
        await Assertions
            .Expect(sectionProfileIdInput)
            .ToHaveValueAsync(sectionProfileId.ToString());

        var sectionProfileRotationInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "section profile rotation" }
        );
        await Assertions
            .Expect(sectionProfileRotationInput)
            .ToHaveValueAsync(sectionProfileRotation.ToString());
    }
}
