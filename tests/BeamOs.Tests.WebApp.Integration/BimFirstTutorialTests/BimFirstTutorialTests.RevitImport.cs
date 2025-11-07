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

        await importData.ClickAsync();

        // Wait a moment for the button to become disabled (indicating import started)
        await this.Page.WaitForTimeoutAsync(100);

        // Wait for the import dialog to disappear
        await this.Expect(importData).Not.ToBeVisibleAsync(new() { Timeout = 3_000 });

        var modelProposalsAfter = await AssemblySetup
            .BeamOsResultApiClient.Models[this.ModelId]
            .Proposals.GetModelProposalsAsync();
        modelProposalsAfter.ThrowIfError();

        modelProposalsAfter.Value.Count.Should().Be(1);

        await this.ViewModelProposal_ShouldSucceed();
        await this.AcceptModelProposal_ShouldSucceed();
    }

    private async Task ViewModelProposal_ShouldSucceed()
    {
        // select the model proposal tab
        var modelProposalsTab = this.Page.GetByRole(
            AriaRole.Button,
            new() { Name = "model proposals" }
        );
        await modelProposalsTab.ClickAsync();

        // select the model proposal
        var modelProposalButton = this.Page.GetByRole(
            AriaRole.Button,
            new() { NameRegex = new System.Text.RegularExpressions.Regex(@"^Proposal \d+$") }
        );
        await modelProposalButton.ClickAsync();

        // todo: assert that the model proposal is displayed in the 3D view
    }

    private async Task AcceptModelProposal_ShouldSucceed()
    {
        // Click next/done to complete the tutorial
        var nextStep = this.Page.GetByRole(AriaRole.Button, new() { Name = "next" })
            .Or(this.Page.GetByRole(AriaRole.Button, new() { Name = "done" }));
        await nextStep.ClickAsync();

        // Wait for the driver to close completely
        await this.Page.WaitForTimeoutAsync(2000);

        // Use JavaScript to click the accept button to bypass any overlay
        await this.Page.EvaluateAsync(@"
            const acceptButton = document.querySelector('[aria-label=""accept""]');
            if (acceptButton) acceptButton.click();
        ");

        // Wait for the confirmation dialog to appear
        await this.Page.WaitForTimeoutAsync(500);

        // Click the Accept button in the confirmation dialog using JavaScript
        await this.Page.EvaluateAsync(@"
            const buttons = Array.from(document.querySelectorAll('button'));
            const acceptButton = buttons.find(b => b.textContent.trim() === 'Accept');
            if (acceptButton) acceptButton.click();
        ");

        // Wait for the dialog to close and API call to complete
        await this.Page.WaitForTimeoutAsync(3000);

        var modelProposals = await AssemblySetup
            .BeamOsResultApiClient.Models[this.ModelId]
            .Proposals.GetModelProposalsAsync();
        modelProposals.ThrowIfError();
        modelProposals.Value.Count.Should().Be(0);

        var model = await AssemblySetup.BeamOsResultApiClient.Models[this.ModelId].GetModelAsync();
        model.ThrowIfError();
        model.Value.Nodes.Should().NotBeEmpty();
        model.Value.Element1ds.Should().NotBeEmpty();
    }
}
