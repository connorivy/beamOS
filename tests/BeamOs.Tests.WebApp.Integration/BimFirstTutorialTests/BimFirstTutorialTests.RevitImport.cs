using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using FluentAssertions;
using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration.BimFirstTutorialTests;

public partial class BimFirstTutorialTests : ReactPageTest
{
    [Test]
    public async Task RevitImport_ShouldSucceed()
    {
        // there should be a dialog on the tutorial page with a stepper that guides the user through the Revit import process
        var dialog = this.Page.GetByRole(AriaRole.Dialog);
        await this.Expect(dialog).ToBeVisibleAsync();

        var nextStep = dialog.GetByRole(
            AriaRole.Button,
            new LocatorGetByRoleOptions { Name = "next" }
        );

        await nextStep.ClickAsync();

        var modelResponseBefore = await AssemblySetup
            .BeamOsResultApiClient.Models[this.ModelId]
            .GetModelAsync();
        modelResponseBefore.ThrowIfError();
        // modelResponseBefore.Value.Metadata.GetTutorialStepNumber().Should().Be(2);

        modelResponseBefore.Value.Nodes.Count.Should().Be(0);

        var importData = dialog.GetByRole(
            AriaRole.Button,
            new LocatorGetByRoleOptions { Name = "import" }
        );
        await importData.ClickAsync();

        var modelResponseAfter = await AssemblySetup
            .BeamOsResultApiClient.Models[this.ModelId]
            .GetModelAsync();
        modelResponseAfter.ThrowIfError();

        modelResponseAfter.Value.Nodes.Count.Should().BeGreaterThanOrEqualTo(3);
    }
}
