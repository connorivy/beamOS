using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.DsmNodeAggregate.ValueObjects;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.DirectStiffnessMethod.DsmNodeAggregate;

public class DsmNode : AggregateRoot<DsmNodeId>
{
    public Point LocationPoint { get; private set; }

    public DsmNode(
        Point locationPoint,
        Restraint restraint,
        List<PointLoad>? pointLoads = null,
        List<MomentLoad>? momentLoads = null,
        DsmNodeId? id = null
    )
        : base(id ?? new())
    {
        this.LocationPoint = locationPoint;
        this.Restraint = restraint;
        this.PointLoads = pointLoads ?? [];
        this.MomentLoads = momentLoads ?? [];
    }

    public DsmNode(
        Length xCoordinate,
        Length yCoordinate,
        Length zCoordinate,
        Restraint restraint,
        List<PointLoad>? pointLoads = null,
        List<MomentLoad>? momentLoads = null,
        DsmNodeId? id = null
    )
        : this(
            new Point(xCoordinate, yCoordinate, zCoordinate),
            restraint,
            pointLoads,
            momentLoads,
            id
        ) { }

    public DsmNode(
        double xCoordinate,
        double yCoordinate,
        double zCoordinate,
        LengthUnit lengthUnit,
        Restraint restraint,
        List<PointLoad>? pointLoads = null,
        List<MomentLoad>? momentLoads = null,
        DsmNodeId? id = null
    )
        : this(
            new Length(xCoordinate, lengthUnit),
            new Length(yCoordinate, lengthUnit),
            new Length(zCoordinate, lengthUnit),
            restraint,
            pointLoads,
            momentLoads,
            id
        ) { }

    public List<PointLoad> PointLoads { get; private set; } = [];
    public List<MomentLoad> MomentLoads { get; } = [];
    public Restraint Restraint { get; set; }

    //public Forces? Forces { get; set; }

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
            forceAlongX += linearLoad.Magnitude * linearLoad.NormalizedDirection[0];
            forceAlongY += linearLoad.Magnitude * linearLoad.NormalizedDirection[1];
            forceAlongZ += linearLoad.Magnitude * linearLoad.NormalizedDirection[2];
        }
        foreach (var momentLoad in this.MomentLoads)
        {
            momentAboutX += momentLoad.Torque * momentLoad.NormalizedAxisDirection[0];
            momentAboutY += momentLoad.Torque * momentLoad.NormalizedAxisDirection[1];
            momentAboutZ += momentLoad.Torque * momentLoad.NormalizedAxisDirection[2];
        }
        return new(forceAlongX, forceAlongY, forceAlongZ, momentAboutX, momentAboutY, momentAboutZ);
    }

    public VectorIdentified GetForceVectorIdentifiedInGlobalCoordinates(
        ForceUnit forceUnit,
        TorqueUnit torqueUnit
    )
    {
        return new(
            this.Id.GetUnsupportedStructureDisplacementIds().ToList(),
            this.GetForcesInGlobalCoordinates().ToArray(forceUnit, torqueUnit)
        );
    }
}
