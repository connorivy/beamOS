using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.IntegrationEvents.PhysicalModel.Nodes;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.PhysicalModel.NodeAggregate;

public class Node : AggregateRoot<NodeId>
{
    public ModelId ModelId { get; private set; }

    private Point locationPoint;
    public Point LocationPoint
    {
        get => this.locationPoint;
        set
        {
            if (this.locationPoint != null)
            {
                this.AddEvent(
                    new NodeMovedEvent
                    {
                        NodeId = this.Id.Id,
                        PreviousLocation = new(
                            this.locationPoint.XCoordinate.Meters,
                            this.locationPoint.YCoordinate.Meters,
                            this.locationPoint.ZCoordinate.Meters
                        ),
                        NewLocation = new(
                            value.XCoordinate.Meters,
                            value.YCoordinate.Meters,
                            value.ZCoordinate.Meters
                        )
                    }
                );
            }
            this.locationPoint = value;
        }
    }

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
    private Restraint restraint;
    public Restraint Restraint
    {
        get => this.restraint;
        set
        {
            //this.AddEvent(
            //    new NodeMovedEvent
            //    {
            //        NodeId = this.Id.Id,
            //        PreviousLocation = new(
            //            this.locationPoint.XCoordinate.Meters,
            //            this.locationPoint.XCoordinate.Meters,
            //            this.locationPoint.XCoordinate.Meters
            //        ),
            //        NewLocation = new(
            //            value.XCoordinate.Meters,
            //            value.YCoordinate.Meters,
            //            value.ZCoordinate.Meters
            //        )
            //    }
            //);
            this.restraint = value;
        }
    }

    public ICollection<PointLoad> PointLoads { get; private set; } = [];
    public ICollection<MomentLoad> MomentLoads { get; private set; } = [];

    public Forces GetForcesInGlobalCoordinates()
    {
        var forceAlongX = Force.Zero;
        var forceAlongY = Force.Zero;
        var forceAlongZ = Force.Zero;
        var momentAboutX = Torque.Zero;
        var momentAboutY = Torque.Zero;
        var momentAboutZ = Torque.Zero;

        foreach (var linearLoad in this.PointLoads)
        {
            forceAlongX += linearLoad.Force * linearLoad.Direction.X;
            forceAlongY += linearLoad.Force * linearLoad.Direction.Y;
            forceAlongZ += linearLoad.Force * linearLoad.Direction.Z;
        }
        foreach (var momentLoad in this.MomentLoads)
        {
            momentAboutX += momentLoad.Torque * momentLoad.AxisDirection[0];
            momentAboutY += momentLoad.Torque * momentLoad.AxisDirection[1];
            momentAboutZ += momentLoad.Torque * momentLoad.AxisDirection[2];
        }
        return new(forceAlongX, forceAlongY, forceAlongZ, momentAboutX, momentAboutY, momentAboutZ);
    }

    public NodeData GetData()
    {
        return new(this.LocationPoint, this.Restraint);
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Node() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
