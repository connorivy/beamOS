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

        await FillOutSectionProfileSelectionInfo(
            this.Page,
            "Test Section Profile",
            10.5,
            200.0,
            150.0,
            300.0,
            25.0,
            20.0,
            8.0,
            9.0
        );

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
        await AssertSectionProfileDialogValues(
            this.Page,
            "Test Section Profile",
            10.5,
            200.0,
            150.0,
            300.0,
            25.0,
            20.0,
            8.0,
            9.0
        );
    }

    [Test]
    [DependsOn(nameof(SectionProfileDialog_ShouldCreateSectionProfile))]
    public async Task SectionProfileDialog_ShouldModifyExistingSectionProfile()
    {
        // click the sectionProfiles tab in the sidebar
        var sectionProfilesTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "sections" }
        );
        await sectionProfilesTab.ClickAsync();

        await FillOutSectionProfileSelectionInfo(
            this.Page,
            "Modified Section Profile",
            999,
            888,
            777,
            666,
            555,
            444,
            333,
            222,
            sectionProfileId: "1"
        );

        // click the apply button
        var applyButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "apply" });
        await applyButton.ClickAsync();

        var sectionProfileIdCombobox = this.Page.GetByRole(
            AriaRole.Combobox,
            new PageGetByRoleOptions { Name = "id" }
        );
        await this.Expect(sectionProfileIdCombobox).ToHaveValueAsync("1");

        // test the optimistic store changes by reloading the element data without refreshing the page
        await sectionProfileIdCombobox.ClickAsync();
        var clearButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "clear" });
        await clearButton.ClickAsync();

        await sectionProfileIdCombobox.FillAsync("1");
        var dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);
        await dropdownOptions.First.ClickAsync();

        await AssertSectionProfileDialogValues(
            this.Page,
            "Modified Section Profile",
            999,
            888,
            777,
            666,
            555,
            444,
            333,
            222,
            sectionProfileId: "1"
        );

        // test the database changes by reloading the element data from the server by refreshing the page
        await this.Page.ReloadAsync();

        // click the sectionProfiles tab in the sidebar again
        sectionProfilesTab = this.Page.GetByRole(
            AriaRole.Button,
            new PageGetByRoleOptions { Name = "sectionProfiles" }
        );
        await sectionProfilesTab.ClickAsync();

        // insert 1 into the sectionProfile id combobox again
        await sectionProfileIdCombobox.FillAsync("1");
        await sectionProfileIdCombobox.ClickAsync();

        // now there should be one result in the dropdown
        dropdownOptions = this.Page.GetByRole(
            AriaRole.Option,
            new PageGetByRoleOptions { Name = "1" }
        );
        await this.Expect(dropdownOptions).ToHaveCountAsync(1);

        // select the sectionProfile from the dropdown
        await dropdownOptions.First.ClickAsync();

        await AssertSectionProfileDialogValues(
            this.Page,
            "Modified Section Profile",
            999,
            888,
            777,
            666,
            555,
            444,
            333,
            222,
            sectionProfileId: "1"
        );
    }

    internal static async Task FillOutSectionProfileSelectionInfo(
        IPage page,
        string name,
        double area,
        double strongAxisMomentOfInertia,
        double weakAxisMomentOfInertia,
        double polarMomentOfInertia,
        double strongAxisPlasticSectionModulus,
        double weakAxisPlasticSectionModulus,
        double weakAxisShearArea,
        double strongAxisShearArea,
        string? sectionProfileId = null
    )
    {
        if (sectionProfileId is not null)
        {
            var sectionProfileIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "id" });
            await sectionProfileIdInput.FillAsync(sectionProfileId);

            // select the option in the dropdown
            var dropdownOption = page.GetByRole(AriaRole.Option, new() { Name = sectionProfileId });
            await dropdownOption.ClickAsync();
        }

        // fill in name
        var nameInput = page.GetByRole(AriaRole.Textbox, new() { Name = "name" });
        await nameInput.FillAsync(name);

        // fill in area
        var areaInput = page.GetByRole(AriaRole.Textbox, new() { Name = "area", Exact = true });
        await areaInput.FillAsync(area.ToString());

        // fill in strong axis moment of inertia
        var strongAxisInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "strong axis moment of inertia" }
        );
        await strongAxisInput.FillAsync(strongAxisMomentOfInertia.ToString());

        // fill in weak axis moment of inertia
        var weakAxisInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "weak axis moment of inertia" }
        );
        await weakAxisInput.FillAsync(weakAxisMomentOfInertia.ToString());

        // fill in polar moment of inertia
        var polarInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "polar moment of inertia" }
        );
        await polarInput.FillAsync(polarMomentOfInertia.ToString());

        // fill in strong axis plastic section modulus
        var strongAxisPlasticInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "strong axis plastic section modulus" }
        );
        await strongAxisPlasticInput.FillAsync(strongAxisPlasticSectionModulus.ToString());

        // fill in weak axis plastic section modulus
        var weakAxisPlasticInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "weak axis plastic section modulus" }
        );
        await weakAxisPlasticInput.FillAsync(weakAxisPlasticSectionModulus.ToString());

        // fill in weak axis shear area
        var shearAreaInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "weak axis shear area" }
        );
        await shearAreaInput.FillAsync(weakAxisShearArea.ToString());

        // fill in strong axis shear area
        var strongShearAreaInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "strong axis shear area" }
        );
        await strongShearAreaInput.FillAsync(strongAxisShearArea.ToString());
    }

    internal static async Task AssertSectionProfileDialogValues(
        IPage page,
        string name,
        double area,
        double strongAxisMomentOfInertia,
        double weakAxisMomentOfInertia,
        double polarMomentOfInertia,
        double strongAxisPlasticSectionModulus,
        double weakAxisPlasticSectionModulus,
        double weakAxisShearArea,
        double strongAxisShearArea,
        string? sectionProfileId = null
    )
    {
        if (sectionProfileId is not null)
        {
            var sectionProfileIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "id" });
            await Assertions.Expect(sectionProfileIdInput).ToHaveValueAsync(sectionProfileId);
        }

        var nameInput = page.GetByRole(AriaRole.Textbox, new() { Name = "name" });
        await Assertions.Expect(nameInput).ToHaveValueAsync(name);

        var areaInput = page.GetByRole(AriaRole.Textbox, new() { Name = "area", Exact = true });
        await areaInput.ExpectToHaveApproximateValueAsync(area);

        var strongAxisInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "strong axis moment of inertia" }
        );
        await strongAxisInput.ExpectToHaveApproximateValueAsync(strongAxisMomentOfInertia);

        var weakAxisInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "weak axis moment of inertia" }
        );
        await weakAxisInput.ExpectToHaveApproximateValueAsync(weakAxisMomentOfInertia);

        var polarInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "polar moment of inertia" }
        );
        await polarInput.ExpectToHaveApproximateValueAsync(polarMomentOfInertia);

        var strongAxisPlasticInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "strong axis plastic section modulus" }
        );
        await strongAxisPlasticInput.ExpectToHaveApproximateValueAsync(
            strongAxisPlasticSectionModulus
        );

        var weakAxisPlasticInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "weak axis plastic section modulus" }
        );
        await weakAxisPlasticInput.ExpectToHaveApproximateValueAsync(weakAxisPlasticSectionModulus);

        var shearAreaInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "weak axis shear area" }
        );
        await shearAreaInput.ExpectToHaveApproximateValueAsync(weakAxisShearArea);

        var strongShearAreaInput = page.GetByRole(
            AriaRole.Textbox,
            new() { Name = "strong axis shear area" }
        );
        await strongShearAreaInput.ExpectToHaveApproximateValueAsync(strongAxisShearArea);
    }
}
