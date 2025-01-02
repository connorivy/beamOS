using System.Diagnostics.CodeAnalysis;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;

public record CreateNodeRequest
{
    [SetsRequiredMembers]
    public CreateNodeRequest(
        Point locationPoint,
        Restraint restraint,
        int? id = null,
        Dictionary<string, string>? metadata = null
    )
    {
        this.LocationPoint = locationPoint;
        this.Restraint = restraint;
        this.Id = id;
        this.Metadata = metadata;
    }

    public CreateNodeRequest() { }

    public required Point LocationPoint { get; init; }
    public required Restraint Restraint { get; init; }
    public int? Id { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}
