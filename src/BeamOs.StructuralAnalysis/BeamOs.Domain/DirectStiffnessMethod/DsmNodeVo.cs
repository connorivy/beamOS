using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Common.Extensions;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.DirectStiffnessMethod;

public class DsmNodeVo(
    NodeId nodeId,
    Point locationPoint,
    Restraint restraint,
    List<PointLoad>? pointLoads = null,
    List<MomentLoad>? momentLoads = null
) : BeamOSValueObject
{
    public NodeId NodeId { get; } = nodeId;
    public Point LocationPoint { get; } = locationPoint;
    public Restraint Restraint { get; } = restraint;
    public List<PointLoad> PointLoads { get; } = pointLoads ?? [];
    public List<MomentLoad> MomentLoads { get; } = momentLoads ?? [];

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

    public VectorIdentified GetForceVectorIdentifiedInGlobalCoordinates(
        ForceUnit forceUnit,
        TorqueUnit torqueUnit
    )
    {
        return new(
            this.NodeId.GetUnsupportedStructureDisplacementIds().ToList(),
            this.GetForcesInGlobalCoordinates().ToArray(forceUnit, torqueUnit)
        );
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.NodeId;
        yield return this.LocationPoint;
        yield return this.Restraint;
        yield return this.PointLoads;
        yield return this.MomentLoads;
    }

    public DsmNodeVo()
        : this(null, null, null) { }
}
