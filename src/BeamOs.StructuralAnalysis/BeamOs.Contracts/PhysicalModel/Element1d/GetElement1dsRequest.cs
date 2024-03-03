using FastEndpoints;

namespace BeamOs.Contracts.PhysicalModel.Element1d;

public class GetElement1dsRequest(string modelId, string[]? element1dIds)
{
    [QueryParam]
    public string ModelId { get; } = modelId;

    [QueryParam]
    public string[]? Element1dIds { get; } = element1dIds;
}
