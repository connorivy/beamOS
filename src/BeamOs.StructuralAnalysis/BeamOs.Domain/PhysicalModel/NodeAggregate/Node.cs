using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.PhysicalModel.NodeAggregate;

public class Node : AggregateRoot<NodeId>
{
    public ModelId ModelId { get; private set; }
    public Point LocationPoint { get; set; }

    public Node(
        ModelId modelId,
        Point locationPoint,
        Restraint? restraint = null,
        NodeId? id = null
    )
        : base(id ?? new())
    {
        this.ModelId = modelId;
        this.LocationPoint = locationPoint;
        this.Restraint = restraint ?? Restraint.Free;
    }

    public Node(
        ModelId modelId,
        double xCoordinate,
        double yCoordinate,
        double zCoordinate,
        LengthUnit lengthUnit,
        Restraint? restraint = null,
        NodeId? id = null
    )
        : this(
            modelId,
            new(xCoordinate, yCoordinate, zCoordinate, lengthUnit),
            restraint: restraint,
            id: id ?? new()
        ) { }

    public Node(
        ModelId modelId,
        Length xCoordinate,
        Length yCoordinate,
        Length zCoordinate,
        Restraint? restraint = null,
        NodeId? id = null
    )
        : this(
            modelId,
            new(xCoordinate, yCoordinate, zCoordinate),
            restraint: restraint,
            id: id ?? new()
        ) { }

    //public List<PointLoadId> PointLoadIds { get; private set; } = [];
    //public List<MomentLoad> MomentLoads { get; } = [];
    public Restraint Restraint { get; set; }

    public NodeData GetData()
    {
        return new(this.LocationPoint, this.Restraint);
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Node() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
