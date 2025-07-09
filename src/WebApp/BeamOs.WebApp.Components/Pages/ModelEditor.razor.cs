using System.Collections.Immutable;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.ProposalInfo;
using BeamOs.WebApp.Components.Layout;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Pages;

public partial class ModelEditor : FluxorComponent
{
    [Parameter]
    public Guid ModelId { get; set; }

    [Inject]
    private IState<EditorComponentState> EditorState { get; init; }

    [Inject]
    private IState<EditorPageState> State { get; init; }

    [Inject]
    private IDispatcher dispatcher { get; init; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        dispatcher.Dispatch(new OpenDrawer());
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        this.SubscribeToAction<ChangeSelectionCommand>(action =>
        {
            var selectedObjects = action.SelectedObjects;
            var selectedObject = selectedObjects.Length == 1 ? selectedObjects[0] : null;

            if (selectedObject?.ObjectType == BeamOsObjectType.Element1d)
            {
                this.dispatcher.Dispatch(new ShowPanelContent(PanelContent.Element1dResults));
            }
        });

        this.SubscribeToAction<ProposalInfoState.ModelProposalInfoLoaded>(action =>
        {
            this.dispatcher.Dispatch(new ShowPanelContent(PanelContent.ProposalInfo));
        });

        this.SubscribeToAction<ProposalInfoState.ModelProposalInfosLoaded>(action =>
        {
            if (action.ProposalInfos.Length > 0)
            {
                this.dispatcher.Dispatch(new ShowPanelContent(PanelContent.ProposalInfo));
            }
        });
    }

    private EditorComponent? editorComponent
    {
        get;
        set
        {
            field = value;
            StateHasChanged();
        }
    }

    protected override ValueTask DisposeAsyncCore(bool disposing)
    {
        dispatcher.Dispatch(new CloseDrawer());
        return base.DisposeAsyncCore(disposing);
    }

    public static string GetPanelId(Guid guid) => $"panel-{guid}";

    public static string GetRelativeUrl(Guid guid) => $"/models/{guid}";

    private void GoBack()
    {
        this.dispatcher.Dispatch(new ShowLastPanelContent());
    }

    [FeatureState]
    public record EditorPageState(
        ImmutableStack<PanelContent> PanelContent,
        bool PanelContentLocked
    )
    {
        public EditorPageState()
            : this(ImmutableStack<PanelContent>.Empty, false) { }
    }

    public readonly record struct ShowPanelContent(PanelContent Content);

    public readonly record struct ShowPanelContentIfNone(PanelContent Content);

    public readonly record struct ShowLastPanelContent();

    public readonly record struct PanelContentLockValue(bool IsLocked);

    public static class EditorPageStateReducers
    {
        [ReducerMethod]
        public static EditorPageState Reducer(EditorPageState state, ShowPanelContent action)
        {
            if (state.PanelContentLocked)
            {
                return state; // Do not change the state if the panel content is locked
            }
            if (!state.PanelContent.IsEmpty && state.PanelContent.Peek() == action.Content)
            {
                return state; // No change if the content is already shown
            }
            return state with { PanelContent = state.PanelContent.Push(action.Content) };
        }

        [ReducerMethod]
        public static EditorPageState Reducer(EditorPageState state, ShowPanelContentIfNone action)
        {
            if (state.PanelContentLocked)
            {
                return state; // Do not change the state if the panel content is locked
            }
            if (state.PanelContent.IsEmpty)
            {
                return state with { PanelContent = state.PanelContent.Push(action.Content) };
            }
            return state;
        }

        [ReducerMethod]
        public static EditorPageState Reducer(EditorPageState state, ShowLastPanelContent action)
        {
            if (state.PanelContentLocked)
            {
                return state; // Do not change the state if the panel content is locked
            }
            if (state.PanelContent.IsEmpty)
            {
                return state;
            }
            return state with { PanelContent = state.PanelContent.Pop() };
        }

        [ReducerMethod]
        public static EditorPageState Reducer(EditorPageState state, PanelContentLockValue action)
        {
            return state with { PanelContentLocked = action.IsLocked };
        }
    }

    public enum PanelContent
    {
        None,
        ProposalInfo,
        Element1dResults,
        ModelRepairScenarioCreator,
    }
}
