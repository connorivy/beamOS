using BeamOS.Common.Domain.Enums;
using BeamOS.Common.Domain.Models;

namespace BeamOS.DirectStiffnessMethod.Domain.NodeAggregate.ValueObjects;
public class UnsupportedStructureDisplacement : BeamOSEntity<UnsupportedStructureDisplacementId>
{
    public UnsupportedStructureDisplacement(UnsupportedStructureDisplacementId identifier, NodeId nodeId, CoordinateSystemDirection3D direction) : base(identifier)
    {
        this.NodeId = nodeId;
        this.Direction = direction;
    }

    public UnsupportedStructureDisplacement Create(UnsupportedStructureDisplacementId identifier, CoordinateSystemDirection3D direction)
    {
        return new UnsupportedStructureDisplacement(identifier, NodeId, direction);
    }

    public NodeId NodeId { get; set; }
    public CoordinateSystemDirection3D Direction { get; set; }

}
