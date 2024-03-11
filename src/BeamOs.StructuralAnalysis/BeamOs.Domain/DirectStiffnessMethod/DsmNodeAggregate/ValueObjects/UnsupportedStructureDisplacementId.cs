using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Models;

namespace BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate.ValueObjects;

public class UnsupportedStructureDisplacementId : BeamOSValueObject
{
    public DsmNodeId NodeId { get; }
    public CoordinateSystemDirection3D Direction { get; }

    public UnsupportedStructureDisplacementId(
        DsmNodeId nodeId,
        CoordinateSystemDirection3D direction
    )
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
