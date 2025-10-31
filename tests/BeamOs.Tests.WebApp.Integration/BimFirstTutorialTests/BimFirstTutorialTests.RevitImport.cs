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

        var modelProposalsBefore = await AssemblySetup
            .BeamOsResultApiClient.Models[this.ModelId]
            .Proposals.GetModelProposalsAsync();
        modelProposalsBefore.ThrowIfError();
        // modelProposalsBefore.Value.Metadata.GetTutorialStepNumber().Should().Be(2);

        modelProposalsBefore.Value.Count.Should().Be(0);

        var importData = dialog.GetByRole(
            AriaRole.Button,
            new LocatorGetByRoleOptions { Name = "import" }
        );

        // Click the import button and wait for it to be enabled again (import complete)
        await importData.ClickAsync();

        // Wait a moment for the button to become disabled (indicating import started)
        await this.Page.WaitForTimeoutAsync(100);

        // Wait for the import to complete (button should be enabled again)
        await this.Expect(importData).Not.ToBeDisabledAsync(new() { Timeout = 10_000 });

        var modelProposalsAfter = await AssemblySetup
            .BeamOsResultApiClient.Models[this.ModelId]
            .Proposals.GetModelProposalsAsync();
        modelProposalsAfter.ThrowIfError();

        modelProposalsAfter.Value.Count.Should().Be(1);
    }

    // [Test]
    // [DependsOn(nameof(RevitImport_ShouldSucceed))]
    // public async Task AcceptModelProposal_ShouldSucceed()
    // {
    //     // there should be a dialog on the tutorial page with a stepper that guides the user through the Revit import process
    //     var dialog = this.Page.GetByRole(AriaRole.Dialog);
    //     await this.Expect(dialog).ToBeVisibleAsync();

    //     var nextStep = dialog.GetByRole(
    //         AriaRole.Button,
    //         new LocatorGetByRoleOptions { Name = "next" }
    //     );

    //     await nextStep.ClickAsync();

    //     var modelResponseBefore = await AssemblySetup
    //         .BeamOsResultApiClient.Models[this.ModelId]
    //         .GetModelAsync();
    //     modelResponseBefore.ThrowIfError();
    //     // modelResponseBefore.Value.Metadata.GetTutorialStepNumber().Should().Be(2);

    //     modelResponseBefore.Value.Nodes.Count.Should().Be(0);

    //     var importData = dialog.GetByRole(
    //         AriaRole.Button,
    //         new LocatorGetByRoleOptions { Name = "import" }
    //     );

    //     // Click the import button and wait for it to be enabled again (import complete)
    //     await importData.ClickAsync();

    //     // Wait a moment for the button to become disabled (indicating import started)
    //     await this.Page.WaitForTimeoutAsync(100);

    //     // Wait for the import to complete (button should be enabled again)
    //     await this.Expect(importData).Not.ToBeDisabledAsync(new() { Timeout = 30000 });

    //     var modelResponseAfter = await AssemblySetup
    //         .BeamOsResultApiClient.Models[this.ModelId]
    //         .GetModelAsync();
    //     modelResponseAfter.ThrowIfError();

    //     modelResponseAfter.Value.Nodes.Count.Should().BeGreaterThanOrEqualTo(3);
    // }
}
