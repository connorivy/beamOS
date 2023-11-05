using BeamOS.Common.Domain.Enums;
using BeamOS.Common.Domain.Models;
using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate.ValueObjects;
//public class UnsupportedStructureDisplacementId : SingleValueWrapperValueObject<int>
//{
//    public UnsupportedStructureDisplacementId(int value) : base(value)
//    {
//    }
//}

public class UnsupportedStructureDisplacementId : BeamOSValueObject
{
    public AnalyticalNodeId NodeId { get; }
    public CoordinateSystemDirection3D Direction { get; }
    public UnsupportedStructureDisplacementId(AnalyticalNodeId nodeId, CoordinateSystemDirection3D direction)
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
