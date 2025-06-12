using BeamOs.CodeGen.EditorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.WebApp.EditorCommands;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ProposalInfo;

public partial class ProposalInfo(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClient,
    ISnackbar snackbar
) : FluxorComponent
{
    [Parameter]
    public Guid ModelId { get; set; }

    [Parameter]
    public IEditorApiAlpha EditorApi { get; set; } = default!;

    private IStructuralAnalysisApiClientV1 StructuralAnalysisApiClient { get; set; } =
        structuralAnalysisApiClient;

    private ISnackbar Snackbar { get; set; } = snackbar;

    private List<ModelProposalInfo>? proposalInfos;
    private ModelProposalResponse? selectedProposal;
    private ProposalIssueSeverity minSeverity = ProposalIssueSeverity.Information;

    private ProposalIssueSeverity[] availableSeverities = Enum.GetValues<ProposalIssueSeverity>()
        .Where(s => s != ProposalIssueSeverity.Undefined)
        .ToArray();

    private IEnumerable<ProposalIssue> FilteredIssues =>
        this.selectedProposal?.ProposalIssues == null
            ? []
            : this.selectedProposal.ProposalIssues.Where(i => i.Severity >= this.minSeverity);

    private IReadOnlyList<IEntityProposal> ProposalObjects
    {
        get
        {
            if (this.selectedProposal == null)
            {
                return [];
            }

            return
            [
                .. this.selectedProposal.CreateNodeProposals ?? [],
                .. this.selectedProposal.ModifyNodeProposals ?? [],
                // .. this.selectedProposal.CreateElement1dProposals ?? [],
                // .. this.selectedProposal.ModifyElement1dProposals ?? [],
                // .. this.selectedProposal.MaterialProposals ?? [],
                // .. this.selectedProposal.SectionProfileProposals ?? [],
                // .. this.selectedProposal.SectionProfileFromLibraryProposals ?? [],
                // .. this.selectedProposal.PointLoadProposals ?? [],
                // .. this.selectedProposal.MomentLoadProposals ?? [],
                // .. this.selectedProposal.DeleteModelEntityProposals ?? [],
            ];
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        // this.SubscribeToAction<ChangeSelectionCommand>(action =>
        // {
        //     if (action.SelectedObjects?.FirstOrDefault() is { TypeName: "ModelProposal" } selected)
        //     {
        //         this.LoadProposal(selected.Id);
        //     }
        //     else
        //     {
        //         this.selectedProposal = null;
        //         this.EditorApi.ClearModelProposalsAsync();
        //     }
        // });
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var response = await this.StructuralAnalysisApiClient.GetModelProposalsAsync(this.ModelId);
        if (response.IsSuccess)
        {
            this.proposalInfos = response.Value;
        }
        else
        {
            this.Snackbar.Add(response.Error.Description, Severity.Error);
            this.proposalInfos = [];
        }
    }

    private async Task LoadProposal(int proposalId)
    {
        var response = await this.StructuralAnalysisApiClient.GetModelProposalAsync(
            this.ModelId,
            proposalId
        );
        if (response.IsSuccess)
        {
            this.selectedProposal = response.Value;
            await this.EditorApi.DisplayModelProposalAsync(response.Value);
        }
        else
        {
            this.Snackbar.Add("Failed to load selected proposal", Severity.Error);
        }
    }

    private async Task AcceptProposal(int proposalId)
    {
        var response = await this.StructuralAnalysisApiClient.AcceptModelProposalAsync(
            this.ModelId,
            proposalId
        );
        if (response.IsSuccess)
        {
            this.Snackbar.Add("Proposal accepted", Severity.Success);
            this.selectedProposal = null;
            await this.EditorApi.ClearModelProposalsAsync();
        }
        else
        {
            this.Snackbar.Add(response.Error.Description, Severity.Error);
        }
    }

    private async Task RejectProposal(int proposalId)
    {
        var response = await this.StructuralAnalysisApiClient.RejectModelProposalAsync(
            this.ModelId,
            proposalId
        );
        if (response.IsSuccess)
        {
            this.Snackbar.Add("Proposal rejected", Severity.Success);
            this.selectedProposal = null;
            await this.EditorApi.ClearModelProposalsAsync();
        }
        else
        {
            this.Snackbar.Add("Failed to reject proposal", Severity.Error);
        }
    }

    private void ResolveIssue(object issue)
    {
        this.Snackbar.Add("Resolve action not yet implemented.", Severity.Info);
    }

    private string GetRelativeTime(DateTimeOffset lastModified)
    {
        var span = DateTime.UtcNow - lastModified.ToUniversalTime();
        if (span.TotalSeconds < 60)
            return $"{(int)span.TotalSeconds} seconds ago";
        if (span.TotalMinutes < 60)
            return $"{(int)span.TotalMinutes} minutes ago";
        if (span.TotalHours < 24)
            return $"{(int)span.TotalHours} hours ago";
        if (span.TotalDays < 7)
            return $"{(int)span.TotalDays} days ago";
        return lastModified.ToString();
    }
}
