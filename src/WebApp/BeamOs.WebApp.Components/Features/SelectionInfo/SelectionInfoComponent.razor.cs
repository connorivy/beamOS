using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.WebApp.Components.Features.Editor;
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

    private ISelectionInfo? GetBeamOsObjectByIdAndType(int id, BeamOsObjectType type)
    {
        ModelCacheKey cacheKey = new(this.ModelId, type, id);
        IModelEntity? modelEntity = state.Value.GetEntityFromCacheOrDefault(cacheKey);

        if (modelEntity is null)
        {
            //throw new Exception("Could not find selected entity in the cache");
            // element was deleted
            return null;
        }

        List<ISelectionInfo>? resultInfo = null;
        if (state.Value.GetEntityResultsFromCacheOrDefault(cacheKey, 1) is IHasModelId response)
        {
            resultInfo =
            [
                this.selectionInfoFactory.Create(response, response.GetType(), $"Analysis Results"),
            ];
        }

        return this.selectionInfoFactory.Create(
            modelEntity,
            modelEntity.GetType(),
            $"{type} {id}",
            additionalSelectionInfo: resultInfo
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
