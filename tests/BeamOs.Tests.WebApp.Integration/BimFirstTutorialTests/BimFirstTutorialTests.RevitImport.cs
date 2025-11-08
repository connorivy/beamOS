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
            new() { Name = "Model Proposals", Exact = true }
        );
        await modelProposalsTab.ClickAsync();
        // delay for driver js to move to next step
        await Task.Delay(750);

        // select the model proposal
        var modelProposalButton = this.Page.GetByRole(
            AriaRole.Button,
            new() { NameRegex = new System.Text.RegularExpressions.Regex(@"^Proposal \d+$") }
        );
        await modelProposalButton.ClickAsync();
        // delay for driver js to move to next step
        await Task.Delay(750);

        // todo: assert that the model proposal is displayed in the 3D view
    }

    private async Task AcceptModelProposal_ShouldSucceed()
    {
        var nextStep = this.Page.GetByRole(AriaRole.Button, new() { Name = "next" });
        await nextStep.ClickAsync();
        // delay for driver js to move to next step
        await Task.Delay(750);

        var acceptProposalButton = this.Page.GetByRole(AriaRole.Button, new() { Name = "accept" });
        await acceptProposalButton.ClickAsync();
        // delay for driver js to move to next step
        await Task.Delay(750);

        var acceptDialog = this.Page.GetByRole(AriaRole.Dialog);
        await this.Expect(acceptDialog).ToBeVisibleAsync(new() { Timeout = 3_000 });
        var confirmAcceptButton = acceptDialog.GetByRole(
            AriaRole.Button,
            new() { Name = "accept" }
        );
        await confirmAcceptButton.ClickAsync();
        // delay for driver js to move to next step
        await Task.Delay(750);

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
