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
        // await this.AddAnalyticalInfo_ShouldSucceed();
        await this.ImportBimGeometryChanges_ShouldSucceed();
        // await this.ViewSecondModelProposal_ShouldSucceed();
        // await this.AcceptSecondModelProposal_ShouldSucceed();
    }

    private async Task ViewModelProposal_ShouldSucceed()
    {
        // select the model proposal tab

        // need to wait a bit for the driver events to hook into the button
        await Task.Delay(300);

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

    private async Task AddAnalyticalInfo_ShouldSucceed()
    {
        // The "Add Analytical Info" step is a tutorial step that just requires clicking next
        // In a real scenario, this would involve adding loads and supports
        var nextStep = this.Page.GetByRole(AriaRole.Button, new() { Name = "next" });
        await nextStep.ClickAsync();
        // delay for driver js to move to next step
        await Task.Delay(750);

        var modelResponse = await AssemblySetup
            .BeamOsResultApiClient.Models[this.ModelId]
            .GetModelAsync();
        modelResponse.ThrowIfError();
        modelResponse.Value.PointLoads.Should().NotBeEmpty();
        modelResponse.Value.PointLoads.Count.Should().Be(1);
        modelResponse.Value.PointLoads.First().NodeId.Should().Be(1);
    }

    private async Task ImportBimGeometryChanges_ShouldSucceed()
    {
        // The "Import BIM Geometry Changes" step simulates updating the BIM model
        // This involves importing new/changed geometry to create a second proposal
        
        // Wait a bit to ensure the first proposal acceptance has fully completed
        await Task.Delay(500);
        
        var nextStep = this.Page.GetByRole(AriaRole.Button, new() { Name = "next" });
        await nextStep.ClickAsync();
        // delay for driver js to move to next step and for the API call to complete
        await Task.Delay(2000);

        // Verify that a second proposal was created
        var modelProposals = await AssemblySetup
            .BeamOsResultApiClient.Models[this.ModelId]
            .Proposals.GetModelProposalsAsync();
        modelProposals.ThrowIfError();
        modelProposals.Value.Count.Should().Be(1, "a second proposal should be created after importing BIM geometry changes");
    }

    private async Task ViewSecondModelProposal_ShouldSucceed()
    {
        // After importing BIM geometry changes, a second proposal would be created
        // This step involves selecting and viewing that second proposal

        // Wait for the UI to update
        await Task.Delay(300);

        // Click on the proposal dropdown area to view the proposals
        // But DON'T click the accept button - just select the proposal to view it
        var proposalSelect = this.Page.Locator("#model-proposals-select");
        await proposalSelect.ClickAsync();
        
        // delay for driver js to move to next step and for the proposal to load
        await Task.Delay(750);
    }

    private async Task AcceptSecondModelProposal_ShouldSucceed()
    {
        // After viewing the second proposal, accept it to integrate the changes

        // TODO: Add logic to accept the second proposal
        // For now, we'll just add the structure for this step
        await Task.Delay(750);
    }
}
