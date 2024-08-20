using BeamOs.Common.Domain.Models;
using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;

namespace BeamOs.Domain.AnalyticalResults.Common.ValueObjects;

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
