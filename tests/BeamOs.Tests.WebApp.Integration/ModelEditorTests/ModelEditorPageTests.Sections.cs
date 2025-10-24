using BeamOs.Tests.Common;
using BeamOs.Tests.WebApp.Integration.Extensions;
using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration.ModelEditorTests;

public partial class ModelEditorPageTests : ReactPageTest
{
    [Test]
    public async Task SectionProfileDialog_ShouldCreateSectionProfile()
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
}
