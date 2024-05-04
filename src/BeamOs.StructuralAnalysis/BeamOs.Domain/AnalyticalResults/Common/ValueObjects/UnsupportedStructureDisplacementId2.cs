using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;

namespace BeamOs.Domain.AnalyticalResults.Common.ValueObjects;

public class UnsupportedStructureDisplacementId2 : BeamOSValueObject
{
    public NodeId NodeId { get; }
    public CoordinateSystemDirection3D Direction { get; }

    public UnsupportedStructureDisplacementId2(NodeId nodeId, CoordinateSystemDirection3D direction)
    {
        this.NodeId = nodeId;
        this.Direction = direction;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.NodeId;
        yield return this.Direction;
    }
}
