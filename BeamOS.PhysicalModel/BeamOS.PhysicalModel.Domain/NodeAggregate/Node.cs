using BeamOS.Common.Domain.Models;
using BeamOS.Common.Domain.ValueObjects;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.PhysicalModel.Domain.NodeAggregate;
public class Node : AggregateRoot<NodeId>
{
    public ModelId ModelId { get; }
    public Point LocationPoint { get; private set; }
    public Node(
        ModelId modelId,
        double xCoordinate,
        double yCoordinate,
        double zCoordinate,
        LengthUnit lengthUnit,
        Restraints? restraint = null,
        NodeId? id = null) : base(id ?? new())
    {
        this.ModelId = modelId;
        this.LocationPoint = new(xCoordinate, yCoordinate, zCoordinate, lengthUnit);
        this.Restraints = restraint ?? Restraints.Free;
    }

    public Node(
        ModelId modelId,
        Length xCoordinate,
        Length yCoordinate,
        Length zCoordinate,
        Restraints? restraint = null,
        NodeId? id = null) : base(id ?? new())
    {
        this.ModelId = modelId;
        this.LocationPoint = new(xCoordinate, yCoordinate, zCoordinate);
        this.Restraints = restraint ?? Restraints.Free;
    }

    public List<PointLoadId> PointLoadIds { get; } = [];
    //public List<MomentLoad> MomentLoads { get; } = [];
    public Restraints Restraints { get; set; }
}
