using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Node;

namespace BeamOS.WebApp.Client.Caches;

public class AllStructuralAnalysisModelCaches
{
    private readonly Dictionary<
        string,
        StructuralAnalysisModelCache
    > modelIdToModelElementCacheDict = [];

    public StructuralAnalysisModelCache GetOrCreateByModelId(string modelId)
    {
        if (
            !this.modelIdToModelElementCacheDict.TryGetValue(
                modelId,
                out StructuralAnalysisModelCache model
            )
        )
        {
            model = new StructuralAnalysisModelCache(modelId);
            this.modelIdToModelElementCacheDict[modelId] = model;
        }

        return model;
    }

    public void DisposeCache(string modelId)
    {
        this.modelIdToModelElementCacheDict.Remove(modelId);
    }
}

public sealed class StructuralAnalysisModelCache(string modelId)
{
    public Dictionary<string, NodeResponse> NodeIdToNodeResponseDict { get; } = [];
    public Dictionary<string, Element1DResponse> Element1dIdToElement1dResponseDict { get; } = [];
}
