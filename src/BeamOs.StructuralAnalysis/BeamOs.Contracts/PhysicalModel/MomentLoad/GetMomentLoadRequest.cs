using FastEndpoints;

namespace BeamOs.Contracts.PhysicalModel.MomentLoad;

public class GetMomentLoadRequest(string modelId, string[]? momentLoadIds)
{
    [QueryParam]
    public string ModelId { get; } = modelId;

    [QueryParam]
    public string[]? MomentLoadIds { get; } = momentLoadIds;
}
