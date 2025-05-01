using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinationAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

public class Node : BeamOsModelEntity<NodeId>
{
    private Point locationPoint;
    public Point LocationPoint
    {
        get => this.locationPoint;
        set
        {
            //if (this.locationPoint != null)
            //{
            //    this.AddEvent(
            //        new NodeMovedEvent
            //        {
            //            ModelId = this.ModelId,
            //            NodeId = this.Id.Id,
            //            PreviousLocation = new(
            //                this.locationPoint.XCoordinate.Meters,
            //                this.locationPoint.YCoordinate.Meters,
            //                this.locationPoint.ZCoordinate.Meters
            //            ),
            //            NewLocation = new(
            //                value.XCoordinate.Meters,
            //                value.YCoordinate.Meters,
            //                value.ZCoordinate.Meters
            //            ),
            //        }
            //    );
            //}
            this.locationPoint = value;
        }
    }

    //[MapperConstructor]
    public Node(
        ModelId modelId,
        Point locationPoint,
        Restraint? restraint = null,
        NodeId? id = null
    )
        : base(id ?? new(), modelId)
    {
        this.LocationPoint = locationPoint;
        this.Restraint = restraint ?? Restraint.Free;
    }

    //public Node(
    //    ModelId modelId,
    //    double xCoordinate,
    //    double yCoordinate,
    //    double zCoordinate,
    //    LengthUnit lengthUnit,
    //    Restraint? restraint = null,
    //    NodeId? id = null
    //)
    //    : this(
    //        modelId,
    //        new(xCoordinate, yCoordinate, zCoordinate, lengthUnit),
    //        restraint: restraint,
    //        id
    //    ) { }

    //public Node(
    //    ModelId modelId,
    //    Length xCoordinate,
    //    Length yCoordinate,
    //    Length zCoordinate,
    //    Restraint? restraint = null,
    //    NodeId? id = null
    //)
    //    : this(modelId, new(xCoordinate, yCoordinate, zCoordinate), restraint: restraint, id) { }

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

    public ICollection<PointLoad>? PointLoads { get; set; }

    public ICollection<MomentLoad>? MomentLoads { get; set; }

    //public NodeResult? NodeResult { get; private set; }

    public Forces GetForcesInGlobalCoordinates(LoadCombination loadCombination)
    {
        var forceAlongX = Force.Zero;
        var forceAlongY = Force.Zero;
        var forceAlongZ = Force.Zero;
        var momentAboutX = Torque.Zero;
        var momentAboutY = Torque.Zero;
        var momentAboutZ = Torque.Zero;

        foreach (var linearLoad in this.PointLoads)
        {
            forceAlongX += linearLoad.GetScaledForce(
                CoordinateSystemDirection3D.AlongX,
                loadCombination
            );
            forceAlongY += linearLoad.GetScaledForce(
                CoordinateSystemDirection3D.AlongY,
                loadCombination
            );
            forceAlongZ += linearLoad.GetScaledForce(
                CoordinateSystemDirection3D.AlongZ,
                loadCombination
            );
        }
        foreach (var momentLoad in this.MomentLoads)
        {
            momentAboutX += momentLoad.GetScaledTorque(
                CoordinateSystemDirection3D.AboutX,
                loadCombination
            );
            momentAboutY += momentLoad.GetScaledTorque(
                CoordinateSystemDirection3D.AboutY,
                loadCombination
            );
            momentAboutZ += momentLoad.GetScaledTorque(
                CoordinateSystemDirection3D.AboutZ,
                loadCombination
            );
        }
        return new(forceAlongX, forceAlongY, forceAlongZ, momentAboutX, momentAboutY, momentAboutZ);
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Node() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
