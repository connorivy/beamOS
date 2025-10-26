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

        await FillOutMomentLoadSelectionInfo(this.Page, "1", 1, 500.0, 1.0, 0.0, 0.0);

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
        await AssertMomentLoadDialogValues(this.Page, "1", 1, 500.0, 1.0, 0.0, 0.0);
    }

    [Test]
    [DependsOn(nameof(MomentLoadDialog_ShouldCreateMomentLoad))]
    public async Task MomentLoadDialog_ShouldModifyExistingMomentLoad()
    {
        // click the momentLoads tab in the sidebar
        var momentLoadsTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "moment loads" }
        );
        await momentLoadsTab.ClickAsync();

        await FillOutMomentLoadSelectionInfo(this.Page, "1", 1, 111, -1, 0, 0, "1");

        // click the apply button
        var applyButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "apply" });
        await applyButton.ClickAsync();

        var momentLoadIdCombobox = this.Page.GetByRole(
            AriaRole.Combobox,
            new PageGetByRoleOptions { Name = "id" }
        );
        await this.Expect(momentLoadIdCombobox).ToHaveValueAsync("1");

        // test the optimistic store changes by reloading the element data without refreshing the page
        await momentLoadIdCombobox.ClickAsync();
        var clearButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "clear" });
        await clearButton.ClickAsync();

        await momentLoadIdCombobox.FillAsync("1");
        var dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);
        await dropdownOptions.First.ClickAsync();

        await AssertMomentLoadDialogValues(this.Page, "1", 1, 111, -1, 0, 0, "1");

        // test the database changes by reloading the element data from the server by refreshing the page
        await this.Page.ReloadAsync();

        // click the momentLoads tab in the sidebar again
        momentLoadsTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "moment loads" }
        );
        await momentLoadsTab.ClickAsync();

        // insert 1 into the momentLoad id combobox again
        await momentLoadIdCombobox.FillAsync("1");
        await momentLoadIdCombobox.ClickAsync();

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // select the momentLoad from the dropdown
        await dropdownOptions.First.ClickAsync();

        await AssertMomentLoadDialogValues(this.Page, "1", 1, 111, -1, 0, 0, "1");
    }

    internal static async Task FillOutMomentLoadSelectionInfo(
        IPage page,
        string loadCase,
        int nodeId,
        double magnitude,
        double directionX,
        double directionY,
        double directionZ,
        string? momentLoadId = null
    )
    {
        if (momentLoadId is not null)
        {
            var momentLoadIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "id" });
            await momentLoadIdInput.FillAsync(momentLoadId);

            // select the option in the dropdown
            var dropdownOption = page.GetByRole(AriaRole.Option, new() { Name = momentLoadId });
            await dropdownOption.ClickAsync();
        }

        var loadCaseInput = page.GetByRole(AriaRole.Combobox, new() { Name = "load case" });
        await loadCaseInput.FillAsync(loadCase);

        var nodeIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "node" });
        await nodeIdInput.FillAsync(nodeId.ToString());

        var fxInput = page.GetByRole(AriaRole.Textbox, new() { Name = "magnitude" });
        await fxInput.FillAsync(magnitude.ToString());

        var directionInput = page.GetByRole(AriaRole.Textbox, new() { Name = "x" });
        await directionInput.FillAsync(directionX.ToString());

        var directionYInput = page.GetByRole(AriaRole.Textbox, new() { Name = "y" });
        await directionYInput.FillAsync(directionY.ToString());

        var directionZInput = page.GetByRole(AriaRole.Textbox, new() { Name = "z" });
        await directionZInput.FillAsync(directionZ.ToString());
    }

    internal static async Task AssertMomentLoadDialogValues(
        IPage page,
        string loadCase,
        int nodeId,
        double magnitude,
        double directionX,
        double directionY,
        double directionZ,
        string? momentLoadId = null
    )
    {
        if (momentLoadId is not null)
        {
            var momentLoadIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "id" });
            await Assertions.Expect(momentLoadIdInput).ToHaveValueAsync(momentLoadId);
        }

        var loadCaseInput = page.GetByRole(AriaRole.Combobox, new() { Name = "load case" });
        await Assertions.Expect(loadCaseInput).ToHaveValueAsync(loadCase);

        var nodeIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "node" });
        await Assertions.Expect(nodeIdInput).ToHaveValueAsync(nodeId.ToString());

        var fxInput = page.GetByRole(AriaRole.Textbox, new() { Name = "magnitude" });
        await fxInput.ExpectToHaveApproximateValueAsync(magnitude);

        var directionInput = page.GetByRole(AriaRole.Textbox, new() { Name = "x" });
        await directionInput.ExpectToHaveApproximateValueAsync(directionX);

        var directionYInput = page.GetByRole(AriaRole.Textbox, new() { Name = "y" });
        await directionYInput.ExpectToHaveApproximateValueAsync(directionY);

        var directionZInput = page.GetByRole(AriaRole.Textbox, new() { Name = "z" });
        await directionZInput.ExpectToHaveApproximateValueAsync(directionZ);
    }
}
