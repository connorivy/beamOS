using BeamOS.Common.Domain.Models;
using BeamOS.Common.Domain.ValueObjects;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.PhysicalModel.Domain.NodeAggregate;
public class Node : AggregateRoot<NodeId>
{
    public ModelId ModelId { get; private set; }
    public Point LocationPoint { get; private set; }
    public Node(
        ModelId modelId,
        double xCoordinate,
        double yCoordinate,
        double zCoordinate,
        LengthUnit lengthUnit,
        NodeId? id = null) : base(id ?? new())
    {
        this.ModelId = modelId;
        this.LocationPoint = new(xCoordinate, yCoordinate, zCoordinate, lengthUnit);
    }

    public Node(
        ModelId modelId,
        Length xCoordinate,
        Length yCoordinate,
        Length zCoordinate,
        NodeId? id = null) : base(id ?? new())
    {
        this.ModelId = modelId;
        this.LocationPoint = new(xCoordinate, yCoordinate, zCoordinate);
    }

    //public List<PointLoadId> PointLoadIds { get; private set; } = [];
    //public List<MomentLoad> MomentLoads { get; } = [];
    public Restraint Restraints { get; set; } = Restraint.Free;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Node() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
