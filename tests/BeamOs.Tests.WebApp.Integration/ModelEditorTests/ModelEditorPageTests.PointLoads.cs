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

        await FillOutPointLoadSelectionInfo(this.Page, "1", 1, 1000.0, 0, 0, -1);

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
        await AssertPointLoadDialogValues(this.Page, "1", 1, 1000.0, 0, 0, -1);
    }

    [Test]
    [DependsOn(nameof(PointLoadDialog_ShouldCreatePointLoad))]
    public async Task PointLoadDialog_ShouldModifyExistingPointLoad()
    {
        // click the pointLoads tab in the sidebar
        var pointLoadsTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "point loads" }
        );
        await pointLoadsTab.ClickAsync();

        await FillOutPointLoadSelectionInfo(this.Page, "1", 1, 111, -1, 0, 0, "1");

        // click the apply button
        var applyButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "apply" });
        await applyButton.ClickAsync();

        var pointLoadIdCombobox = this.Page.GetByRole(
            AriaRole.Combobox,
            new PageGetByRoleOptions { Name = "id" }
        );
        await this.Expect(pointLoadIdCombobox).ToHaveValueAsync("1");

        // test the optimistic store changes by reloading the element data without refreshing the page
        await pointLoadIdCombobox.ClickAsync();
        var clearButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "clear" });
        await clearButton.ClickAsync();

        await pointLoadIdCombobox.FillAsync("1");
        var dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);
        await dropdownOptions.First.ClickAsync();

        await AssertPointLoadDialogValues(this.Page, "1", 1, 111, -1, 0, 0, "1");

        // test the database changes by reloading the element data from the server by refreshing the page
        await this.Page.ReloadAsync();

        // click the pointLoads tab in the sidebar again
        pointLoadsTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "point loads" }
        );
        await pointLoadsTab.ClickAsync();

        // insert 1 into the pointLoad id combobox again
        await pointLoadIdCombobox.FillAsync("1");
        await pointLoadIdCombobox.ClickAsync();

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // select the pointLoad from the dropdown
        await dropdownOptions.First.ClickAsync();

        await AssertPointLoadDialogValues(this.Page, "1", 1, 111, -1, 0, 0, "1");
    }

    internal static async Task FillOutPointLoadSelectionInfo(
        IPage page,
        string loadCase,
        int nodeId,
        double magnitude,
        double directionX,
        double directionY,
        double directionZ,
        string? pointLoadId = null
    )
    {
        if (pointLoadId is not null)
        {
            var pointLoadIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "id" });
            await pointLoadIdInput.FillAsync(pointLoadId);

            // select the option in the dropdown
            var dropdownOption = page.GetByRole(AriaRole.Option, new() { Name = pointLoadId });
            await dropdownOption.ClickAsync();
        }

        var loadCaseInput = page.GetByRole(AriaRole.Combobox, new() { Name = "load case" });
        await loadCaseInput.FillAsync(loadCase);

        var nodeIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "node" });
        await nodeIdInput.FillAsync(nodeId.ToString());

        var fxInput = page.GetByRole(AriaRole.Textbox, new() { Name = "magnitude" });
        await fxInput.FillAsync(magnitude.ToString());

        var directionInput = page.GetByRole(AriaRole.Combobox, new() { Name = "x" });
        await directionInput.FillAsync(directionX.ToString());

        var directionYInput = page.GetByRole(AriaRole.Combobox, new() { Name = "y" });
        await directionYInput.FillAsync(directionY.ToString());

        var directionZInput = page.GetByRole(AriaRole.Combobox, new() { Name = "z" });
        await directionZInput.FillAsync(directionZ.ToString());
    }

    internal static async Task AssertPointLoadDialogValues(
        IPage page,
        string loadCase,
        int nodeId,
        double magnitude,
        double directionX,
        double directionY,
        double directionZ,
        string? pointLoadId = null
    )
    {
        if (pointLoadId is not null)
        {
            var pointLoadIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "id" });
            await Assertions.Expect(pointLoadIdInput).ToHaveValueAsync(pointLoadId);
        }

        var loadCaseInput = page.GetByRole(AriaRole.Combobox, new() { Name = "load case" });
        await Assertions.Expect(loadCaseInput).ToHaveValueAsync(loadCase);

        var nodeIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "node" });
        await Assertions.Expect(nodeIdInput).ToHaveValueAsync(nodeId.ToString());

        var fxInput = page.GetByRole(AriaRole.Textbox, new() { Name = "magnitude" });
        await fxInput.ExpectToHaveApproximateValueAsync(magnitude);

        var directionInput = page.GetByRole(AriaRole.Combobox, new() { Name = "x" });
        await directionInput.ExpectToHaveApproximateValueAsync(directionX);

        var directionYInput = page.GetByRole(AriaRole.Combobox, new() { Name = "y" });
        await directionYInput.ExpectToHaveApproximateValueAsync(directionY);

        var directionZInput = page.GetByRole(AriaRole.Combobox, new() { Name = "z" });
        await directionZInput.ExpectToHaveApproximateValueAsync(directionZ);
    }
}
