using BeamOs.Common.Contracts;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.StructuralApi;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Features.SelectionInfo;

public partial class SelectionInfoComponent(IState<CachedModelState> state, IDispatcher dispatcher)
    : FluxorComponent
{
    [Parameter]
    public SelectedObject[] SelectedObjects { get; init; }

    [Parameter]
    public required string CanvasId { get; init; }

    [Parameter]
    public required Guid ModelId { get; init; }

    [Parameter]
    public string? Class { get; init; }

    private readonly SelectionInfoFactory selectionInfoFactory = new();

    //private ISelectionInfo GetBeamOsObjectByIdAndTypeName(IModelEntity modelEntity)
    //{
    //    return this.selectionInfoFactory.Create(
    //        modelEntity,
    //        modelEntity.GetType(),
    //        $"{modelEntity.GetType().Name} {modelEntity.Id}"
    //    );
    //}

    private ISelectionInfo? GetBeamOsObjectByIdAndTypeName(int id, string typeName)
    {
        IModelEntity? modelEntity = state
            .Value
            .GetEntityFromCacheOrDefault(new(this.ModelId, typeName, id));

        if (modelEntity is null)
        {
            //throw new Exception("Could not find selected entity in the cache");
            // element was deleted
            return null;
        }

        return this.selectionInfoFactory.Create(
            modelEntity,
            modelEntity.GetType(),
            $"{typeName} {id}"
        );
    }

    //private ISelectionInfo GetBeamOsObjectByIdAndTypeName(int id, string typeName)
    //{
    //    var cachedModelResponse = state.Value.Models[this.ModelId];

    //    object selected = typeName switch
    //    {
    //        "Node" => cachedModelResponse.Nodes[id],
    //        "PointLoad" => cachedModelResponse.PointLoads[id],
    //        "Element1d" => cachedModelResponse.Element1ds[id],
    //        _ => throw new NotImplementedException($"type name, {typeName}, is not implemented")
    //    };

    //    return this.selectionInfoFactory.Create(selected, selected.GetType(), $"{typeName} {id}");
    //}
}
