using BeamOS.Common.Domain.Enums;
using BeamOS.Common.Domain.Models;

namespace BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate.ValueObjects;
public class UnsupportedStructureDisplacement : BeamOSEntity<UnsupportedStructureDisplacementId>
{
    public UnsupportedStructureDisplacement(UnsupportedStructureDisplacementId identifier, AnalyticalNodeId nodeId, CoordinateSystemDirection3D direction) : base(identifier)
    {
        this.NodeId = nodeId;
        this.Direction = direction;
    }

    public UnsupportedStructureDisplacement Create(UnsupportedStructureDisplacementId identifier, CoordinateSystemDirection3D direction)
    {
        return new UnsupportedStructureDisplacement(identifier, NodeId, direction);
    }

    public AnalyticalNodeId NodeId { get; set; }
    public CoordinateSystemDirection3D Direction { get; set; }

}
