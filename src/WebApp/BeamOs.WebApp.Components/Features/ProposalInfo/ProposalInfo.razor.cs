using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using BeamOs.CodeGen.EditorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Pages;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata;
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
            dispatcher.Dispatch(
                new ProposalInfoState.ModelProposalInfosLoaded(response.Value.ToImmutableArray())
            );
        }
        else
        {
            this.Snackbar.Add(response.Error.Detail, Severity.Error);
            dispatcher.Dispatch(new ProposalInfoState.ModelProposalInfosLoaded([]));
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
            proposalId,
            this.unselectedProposalObjectIds.Select(o => new EntityProposal(
                    o.ObjectType,
                    o.Id,
                    o.ProposalType
                ))
                .ToList()
        );
        if (response.IsSuccess)
        {
            this.Snackbar.Add("Proposal accepted", Severity.Success);
            dispatcher.Dispatch(new ProposalInfoState.ModelProposalResponseChanged(null));
            dispatcher.Dispatch(new ProposalInfoState.ModelProposalDeleted(proposalId));
            await this.EditorApi.ClearModelProposalsAsync();
        }
        else
        {
            this.Snackbar.Add(response.Error.Detail, Severity.Error);
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
            dispatcher.Dispatch(new ProposalInfoState.ModelProposalDeleted(proposalId));
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

    private HashSet<IEntityProposal> unselectedProposalObjectIds = new();

    private void ToggleProposalObjectSelection(IEntityProposal entityProposal, bool isChecked)
    {
        if (isChecked == false)
        {
            unselectedProposalObjectIds.Add(entityProposal);
        }
        else
        {
            unselectedProposalObjectIds.Remove(entityProposal);
        }
    }

    private bool? selectAllChecked
    {
        get
        {
            var visibleProposals = state.Value.FilteredProposals;

            if (visibleProposals is null || visibleProposals.Count == 0)
            {
                return false;
            }

            var checkedCount = visibleProposals.Count(this.unselectedProposalObjectIds.Contains);
            if (checkedCount == 0)
            {
                return true;
            }

            if (checkedCount == visibleProposals.Count)
            {
                return false;
            }

            return null;
        }
        set
        {
            if (value.HasValue)
            {
                var visibleIds = state.Value.FilteredProposals;

                if (!value.Value)
                {
                    foreach (var id in visibleIds)
                    {
                        this.unselectedProposalObjectIds.Add(id);
                    }
                }
                else
                {
                    foreach (var id in visibleIds)
                    {
                        this.unselectedProposalObjectIds.Remove(id);
                    }
                }
                this.StateHasChanged();
            }
        }
    }
}

[FeatureState]
public record ProposalInfoState(
    ImmutableArray<ModelProposalInfo>? ProposalInfos,
    ModelProposalResponse? SelectedProposal,
    IReadOnlyList<IEntityProposal>? EntityProposals,
    IReadOnlyDictionary<
        DeleteModelEntityProposalData,
        DeleteModelEntityProposalContract
    >? DeleteEntityProposals,
    IReadOnlyList<IEntityProposal>? FilteredProposals
)
{
    public ProposalInfoState()
        : this(null, null, null, null, null) { }

    public record struct ModelProposalResponseChanged(ModelProposalResponse? SelectedProposal);

    public record struct ModelProposalInfoLoaded(ModelProposalInfo ModelProposalInfo);

    public record struct ModelProposalInfosLoaded(ImmutableArray<ModelProposalInfo> ProposalInfos);

    public record struct ModelProposalDeleted(int ProposalId);
}

public static class ProposalInfoReducers
{
    [ReducerMethod]
    public static ProposalInfoState OnModelProposalInfoLoaded(
        ProposalInfoState state,
        ProposalInfoState.ModelProposalInfoLoaded action
    )
    {
        var newProposalInfos = state.ProposalInfos ?? [];
        return state with { ProposalInfos = newProposalInfos.Add(action.ModelProposalInfo) };
    }

    [ReducerMethod]
    public static ProposalInfoState OnModelProposalInfosLoaded(
        ProposalInfoState state,
        ProposalInfoState.ModelProposalInfosLoaded action
    )
    {
        return state with { ProposalInfos = action.ProposalInfos };
    }

    [ReducerMethod]
    public static ProposalInfoState OnModelProposalDeleted(
        ProposalInfoState state,
        ProposalInfoState.ModelProposalDeleted action
    )
    {
        if (state.ProposalInfos is null)
        {
            return state;
        }

        var newProposalInfos = state.ProposalInfos?.RemoveAll(p => p.Id == action.ProposalId);

        return state with
        {
            ProposalInfos = newProposalInfos,
        };
    }

    [ReducerMethod]
    public static ProposalInfoState OnEntityProposalsLoading(
        ProposalInfoState state,
        ChangeSelectionCommand action
    )
    {
        if (action.SelectedObjects.Length == 0 || state.EntityProposals is null)
        {
            return state with { FilteredProposals = state.EntityProposals };
        }
        Debug.Assert(state.DeleteEntityProposals is not null);

        var deleteProposalCandidates = action
            .SelectedObjects.Where(o => !o.ObjectType.IsProposalType())
            .Select(o =>
                state.DeleteEntityProposals.GetValueOrDefault(
                    new DeleteModelEntityProposalData()
                    {
                        ModelEntityId = o.Id,
                        ObjectType = o.ObjectType,
                    }
                )
            )
            .OfType<DeleteModelEntityProposalContract>();

        // the selected objects are coming from the editor and are going to have the type of 'proposal'
        // whereas the proposals 'objectType' is the actual type that the proposal is going to modify (e.g. Node, Element1d, etc.)
        // so we need to translate the incoming proposal types into the actual proposal types
        var createOrModifyProposals = action
            .SelectedObjects.Where(p => p.ObjectType.IsProposalType())
            .Select(p => new EntityProposal(
                p.ObjectType.ToAffectedType(),
                p.Id,
                ProposalType.Create // todo: this value is only used in the backend to determine
            // if the proposal type is delete. So it doesn't matter if this is correct currently.
            ));

        var selectedObjectsHashSet = deleteProposalCandidates
            .Concat<IEntityProposal>(createOrModifyProposals)
            .ToHashSet();

        var newFilteredProposalObjects = state
            .EntityProposals.Where(o =>
                selectedObjectsHashSet.Contains(o, GenericEntityProposalComparer.Instance)
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
            return state with
            {
                SelectedProposal = null,
                EntityProposals = null,
                FilteredProposals = null,
                DeleteEntityProposals = null,
            };
        }

        List<IEntityProposal> entityProposals =
        [
            // .. action.SelectedProposal.CreateNodeProposals ?? [],
            // .. action.SelectedProposal.ModifyNodeProposals ?? [],
            .. action.SelectedProposal.CreateElement1dProposals ?? [],
            // .. action.SelectedProposal.ModifyElement1dProposals ?? [],
            // .. action.SelectedProposal.MaterialProposals ?? [],
            // .. action.SelectedProposal.SectionProfileProposals ?? [],
            // .. action.SelectedProposal.SectionProfileFromLibraryProposals ?? [],
            // .. action.SelectedProposal.PointLoadProposals ?? [],
            // .. action.SelectedProposal.MomentLoadProposals ?? [],
            .. action.SelectedProposal.DeleteModelEntityProposals ?? [],
        ];
        return state with
        {
            SelectedProposal = action.SelectedProposal,
            EntityProposals = entityProposals,
            DeleteEntityProposals =
                action.SelectedProposal.DeleteModelEntityProposals?.ToImmutableDictionary(p =>
                    p as DeleteModelEntityProposalData
                ),
            FilteredProposals = entityProposals,
        };
    }
}

public class GenericEntityProposalComparer : IEqualityComparer<IEntityProposal>
{
    public static GenericEntityProposalComparer Instance { get; } = new();

    public bool Equals(IEntityProposal? x, IEntityProposal? y)
    {
        if (x is null && y is null)
        {
            return true;
        }
        if (x is null || y is null)
        {
            return false;
        }

        return x.ObjectType == y.ObjectType && x.Id == y.Id;
    }

    public int GetHashCode([DisallowNull] IEntityProposal obj)
    {
        return HashCode.Combine(obj.ObjectType, obj.Id);
    }
}
