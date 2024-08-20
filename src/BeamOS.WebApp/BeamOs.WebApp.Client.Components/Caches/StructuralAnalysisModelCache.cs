using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Node;

namespace BeamOs.WebApp.Client.Components.Caches;

public class AllStructuralAnalysisModelCaches
{
    private readonly Dictionary<
        string,
        SingleStructuralAnalysisModelCache
    > modelIdToModelElementCacheDict = [];

    public SingleStructuralAnalysisModelCache GetByModelId(string modelId) =>
        this.modelIdToModelElementCacheDict[modelId];

    public SingleStructuralAnalysisModelCache GetOrCreateByModelId(string modelId)
    {
        if (
            !this.modelIdToModelElementCacheDict.TryGetValue(
                modelId,
                out SingleStructuralAnalysisModelCache model
            )
        )
        {
            model = new SingleStructuralAnalysisModelCache();
            this.modelIdToModelElementCacheDict[modelId] = model;
        }

        return model;
    }

    public void DisposeCache(string modelId)
    {
        this.modelIdToModelElementCacheDict.Remove(modelId);
    }
}

public sealed class SingleStructuralAnalysisModelCache
{
    private readonly Dictionary<string, BeamOsEntityContractBase> cachedEntities = [];
    private readonly Dictionary<Type, List<string>> entityTypeToCachedEntityIdsDict = [];

    public void AddOrReplace(BeamOsEntityContractBase element)
    {
        Type elementType = element.GetType();
        this.cachedEntities[element.Id] = element;

        if (!this.entityTypeToCachedEntityIdsDict.TryGetValue(elementType, out var idList))
        {
            idList =  [element.Id];
            this.entityTypeToCachedEntityIdsDict[elementType] = idList;
        }
        else if (!idList.Contains(element.Id))
        {
            idList.Add(element.Id);
        }
    }

    public bool TryGetById<T>(string id, out T? entity)
        where T : BeamOsEntityContractBase
    {
        var returnVal = this.cachedEntities.TryGetValue(
            id,
            out BeamOsEntityContractBase? cachedEntity
        );
        entity = (T)cachedEntity;
        return returnVal;
    }

    public T GetById<T>(string id)
        where T : BeamOsEntityContractBase
    {
        return (T)this.cachedEntities[id]
            ?? throw new Exception($"Could not find resource with id = {id}");
    }

    public List<string> GetEntityIdsOfType<T>()
    {
        return this.entityTypeToCachedEntityIdsDict[typeof(T)]
            ?? throw new Exception($"Could not find any entities of type {typeof(T)}");
    }
}

public sealed class StructuralAnalysisModelCache(string modelId)
{
    public Dictionary<string, NodeResponse> NodeIdToNodeResponseDict { get; } = [];
    public Dictionary<string, Element1DResponse> Element1dIdToElement1dResponseDict { get; } = [];
}
