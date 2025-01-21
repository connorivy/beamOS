using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Features.SelectionInfo;

public partial class SelectionInfoComponent(
    IStateSelection<AllEditorComponentState, EditorComponentState> state
) : FluxorComponent
{
    public SelectedObject[] SelectedObjects => state.Value.SelectedObjects;

    [Parameter]
    public required string CanvasId { get; init; }

    [Parameter]
    public string? Class { get; init; }

    //[Inject]
    //private AllStructuralAnalysisModelCaches AllStructuralAnalysisModelCaches { get; init; }

    private object GetBeamOsObjectByIdAndTypeName(int id, string typeName)
    {
        var cachedModelResponse = state.Value.CachedModelResponse;
        if (cachedModelResponse is null)
        {
            return null;
        }

        return typeName switch
        {
            "Node" => cachedModelResponse.Nodes[id],
            "PointLoad" => cachedModelResponse.PointLoads[id],
            "Element1d" => cachedModelResponse.Element1ds[id]
        };
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        state.Select(s => s.EditorState[this.CanvasId]);

        //this.SubscribeToAction<MoveNodeCommand>(command =>
        //{
        //    if (command.CanvasId == this.CanvasId)
        //    {
        //        this.StateHasChanged();
        //    }
        //});
    }

    //private object GetAdditionalObjectInfo(string id, string typeName)
    //{
    //    return this.AllStructuralAnalysisModelCaches.GetByModelId(this.ModelId).GetById<BeamOsEntityContractBase>(NodeResultResponseEntity.ResultId(id));
    //}
}
