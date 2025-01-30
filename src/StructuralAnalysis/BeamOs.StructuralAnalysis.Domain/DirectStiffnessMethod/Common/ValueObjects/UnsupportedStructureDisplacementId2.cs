using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.ValueObjects;

public class UnsupportedStructureDisplacementId2(
    NodeId nodeId,
    CoordinateSystemDirection3D direction
) : BeamOSValueObject
{
    public NodeId NodeId { get; } = nodeId;
    public CoordinateSystemDirection3D Direction { get; } = direction;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.NodeId;
        yield return this.Direction;
    }
}
