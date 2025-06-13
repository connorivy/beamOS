using BeamOs.CodeGen.EditorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ProposalInfo;

public partial class ProposalInfo(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClient,
    ISnackbar snackbar,
    IState<ProposalInfoState> state,
    IDispatcher dispatcher
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
    private ProposalIssueSeverity minSeverity = ProposalIssueSeverity.Information;

    private ProposalIssueSeverity[] availableSeverities = Enum.GetValues<ProposalIssueSeverity>()
        .Where(s => s != ProposalIssueSeverity.Undefined)
        .ToArray();

    private IEnumerable<ProposalIssue> FilteredIssues =>
        state.Value.SelectedProposal?.ProposalIssues == null
            ? []
            : state.Value.SelectedProposal.ProposalIssues.Where(i =>
                i.Severity >= this.minSeverity
            );

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
            dispatcher.Dispatch(new ProposalInfoState.ModelProposalResponseChanged(response.Value));

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
            dispatcher.Dispatch(new ProposalInfoState.ModelProposalResponseChanged(null));
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
            dispatcher.Dispatch(new ProposalInfoState.ModelProposalResponseChanged(null));
            await this.EditorApi.ClearModelProposalsAsync();
        }
        else
        {
            this.Snackbar.Add("Failed to reject proposal", Severity.Error);
        }
    }

    private void GoBack()
    {
        dispatcher.Dispatch(new ProposalInfoState.ModelProposalResponseChanged(null));
    }

    private void ResolveIssue(object issue)
    {
        this.Snackbar.Add("Resolve action not yet implemented.", Severity.Info);
    }

    private string GetRelativeTime(DateTimeOffset lastModified)
    {
        var span = DateTime.UtcNow - lastModified.ToUniversalTime();
        if (span.TotalSeconds < 60)
        {
            return $"{(int)span.TotalSeconds} seconds ago";
        }

        if (span.TotalMinutes < 60)
        {
            return $"{(int)span.TotalMinutes} minutes ago";
        }

        if (span.TotalHours < 24)
        {
            return $"{(int)span.TotalHours} hours ago";
        }

        if (span.TotalDays < 7)
        {
            return $"{(int)span.TotalDays} days ago";
        }

        return lastModified.ToString();
    }

    private HashSet<string> selectedProposalObjectIds = new();

    private void ToggleProposalObjectSelection(string id, bool isChecked)
    {
        if (isChecked == true)
        {
            selectedProposalObjectIds.Add(id);
        }
        else
        {
            selectedProposalObjectIds.Remove(id);
        }
    }
}

[FeatureState]
public record ProposalInfoState(
    ModelProposalResponse? SelectedProposal,
    IReadOnlyDictionary<ModelEntityId, IEntityProposal>? EntityProposals,
    IReadOnlyList<IEntityProposal>? FilteredProposals
)
{
    public ProposalInfoState()
        : this(null, null, null) { }

    public record struct ModelProposalResponseChanged(ModelProposalResponse? SelectedProposal);

    public record struct EntityProposalsLoaded(
        IReadOnlyDictionary<ModelEntityId, IEntityProposal> EntityProposals
    );

    public record struct FilteredProposalsChanged(IReadOnlyList<IEntityProposal> FilteredProposals);
}

public static class ProposalInfoReducers
{
    [ReducerMethod]
    public static ProposalInfoState OnEntityProposalsLoading(
        ProposalInfoState state,
        ChangeSelectionCommand action
    )
    {
        var selectedObjectsHashSet = new HashSet<ModelEntityId>(
            action.SelectedObjects.Select(o => new ModelEntityId(o.ObjectType, o.Id))
        );
        var newFilteredProposalObjects = state
            .EntityProposals?.Values.Where(p =>
                selectedObjectsHashSet.Contains(new ModelEntityId(p.ObjectType, p.Id))
            )
            .ToList();

        return state with
        {
            FilteredProposals = newFilteredProposalObjects,
        };
    }

    [ReducerMethod]
    public static ProposalInfoState OnSelectedProposalChanged(
        ProposalInfoState state,
        ProposalInfoState.ModelProposalResponseChanged action
    )
    {
        if (action.SelectedProposal == null)
        {
            return state with { SelectedProposal = null, EntityProposals = null };
        }

        IEnumerable<IEntityProposal> proposals =
        [
            .. action.SelectedProposal.CreateNodeProposals ?? [],
            .. action.SelectedProposal.ModifyNodeProposals ?? [],
            // .. action.SelectedProposal.CreateElement1dProposals ?? [],
            // .. action.SelectedProposal.ModifyElement1dProposals ?? [],
            // .. action.SelectedProposal.MaterialProposals ?? [],
            // .. action.SelectedProposal.SectionProfileProposals ?? [],
            // .. action.SelectedProposal.SectionProfileFromLibraryProposals ?? [],
            // .. action.SelectedProposal.PointLoadProposals ?? [],
            // .. action.SelectedProposal.MomentLoadProposals ?? [],
            // .. action.SelectedProposal.DeleteModelEntityProposals ?? [],
        ];
        var entityProposals = proposals.ToDictionary(p => new ModelEntityId(p.ObjectType, p.Id));
        return state with
        {
            SelectedProposal = action.SelectedProposal,
            EntityProposals = entityProposals,
        };
    }

    [ReducerMethod]
    public static ProposalInfoState OnEntityProposalsLoaded(
        ProposalInfoState state,
        ProposalInfoState.EntityProposalsLoaded action
    )
    {
        return state with { EntityProposals = action.EntityProposals };
    }

    [ReducerMethod]
    public static ProposalInfoState OnFilteredProposalsChanged(
        ProposalInfoState state,
        ProposalInfoState.FilteredProposalsChanged action
    )
    {
        return state with { FilteredProposals = action.FilteredProposals };
    }
}
