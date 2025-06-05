using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

public record InternalNodeData
{
    [SetsRequiredMembers]
    public InternalNodeData(
        int element1dId,
        Ratio ratioAlongElement1d,
        Dictionary<string, string>? metadata = null
    )
    {
        this.Element1dId = element1dId;
        if (ratioAlongElement1d.As(RatioUnit.DecimalFraction) is < 0 or > 1)
        {
            throw new ArgumentException("Ratio along element must be between 0 and 1");
        }

        this.RatioAlongElement1d = ratioAlongElement1d;
        this.Metadata = metadata;
    }

    public required int Element1dId { get; init; }
    public required Ratio RatioAlongElement1d { get; init; }

    // public InternalNodeData() { }

    public Dictionary<string, string>? Metadata { get; init; }
}

public record CreateInternalNodeRequest : InternalNodeData
{
    [SetsRequiredMembers]
    public CreateInternalNodeRequest(
        int element1dId,
        Ratio ratioAlongElement1d,
        int? id = null,
        Dictionary<string, string>? metadata = null
    )
        : base(element1dId, ratioAlongElement1d, metadata)
    {
        this.Id = id;
    }

    public int? Id { get; init; }
}

public record InternalNode : InternalNodeData, IHasIntId
{
    [SetsRequiredMembers]
    public InternalNode(
        int id,
        int element1dId,
        Ratio ratioAlongElement1d,
        Dictionary<string, string>? metadata = null
    )
        : base(element1dId, ratioAlongElement1d, metadata)
    {
        this.Id = id;
    }

    public required int Id { get; init; }
}
