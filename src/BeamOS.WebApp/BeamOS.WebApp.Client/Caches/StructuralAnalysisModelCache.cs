using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Node;

namespace BeamOS.WebApp.Client.Caches;

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
    private readonly Dictionary<Type, Dictionary<string, BeamOsEntityContractBase>> elementStore =
    [];

    public void AddOrReplace<T>(T element)
        where T : BeamOsEntityContractBase
    {
        if (!this.elementStore.TryGetValue(typeof(T), out var subStore))
        {
            subStore =  [];
            this.elementStore.Add(typeof(T), subStore);
        }

        subStore[element.Id] = element;
    }

    public T GetById<T>(string id)
        where T : BeamOsEntityContractBase
    {
        if (this.elementStore.TryGetValue(typeof(T), out var subStore))
        {
            return (T)subStore[id];
        }

        throw new Exception($"Could not find resource with id = {id}");
    }
}

public sealed class StructuralAnalysisModelCache(string modelId)
{
    public Dictionary<string, NodeResponse> NodeIdToNodeResponseDict { get; } = [];
    public Dictionary<string, Element1DResponse> Element1dIdToElement1dResponseDict { get; } = [];
}
