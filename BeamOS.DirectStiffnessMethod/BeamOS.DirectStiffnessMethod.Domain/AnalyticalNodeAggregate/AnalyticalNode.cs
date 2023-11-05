using BeamOS.Common.Domain.Enums;
using BeamOS.Common.Domain.Models;
using BeamOS.Common.Domain.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.Common.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;
public class AnalyticalNode : AggregateRoot<AnalyticalNodeId>
{
    public Point LocationPoint { get; private set; }
    private AnalyticalNode(
        AnalyticalNodeId id,
        double xCoordinate,
        double yCoordinate,
        double zCoordinate,
        LengthUnit lengthUnit,
        Restraints? restraint = null) : base(id)
    {
        this.LocationPoint = new(xCoordinate, yCoordinate, zCoordinate, lengthUnit);
        this.Restraints = restraint ?? Restraints.Free;
    }
    public static AnalyticalNode Create(
        double xCoordinate,
        double yCoordinate,
        double zCoordinate,
        LengthUnit lengthUnit,
        Restraints? restraint = null
    )
    {
        return new(AnalyticalNodeId.CreateUnique(), xCoordinate, yCoordinate, zCoordinate, lengthUnit, restraint);
    }
    private AnalyticalNode(
        AnalyticalNodeId id,
        Length xCoordinate,
        Length yCoordinate,
        Length zCoordinate,
        Restraints? restraint = null) : base(id)
    {
        this.LocationPoint = new(xCoordinate, yCoordinate, zCoordinate);
        this.Restraints = restraint ?? Restraints.Free;
    }
    public static AnalyticalNode Create(
        Length xCoordinate,
        Length yCoordinate,
        Length zCoordinate,
        Restraints? restraint = null
    )
    {
        return new(AnalyticalNodeId.CreateUnique(), xCoordinate, yCoordinate, zCoordinate, restraint);
    }

    public List<LinearLoad> LinearLoads { get; } = [];
    public List<MomentLoad> MomentLoads { get; } = [];
    public Restraints Restraints { get; set; }
    public Forces? Forces { get; set; }

    public Forces GetForcesInGlobalCoordinates()
    {
        Force forceAlongX = Force.Zero;
        Force forceAlongY = Force.Zero;
        Force forceAlongZ = Force.Zero;
        Torque momentAboutX = Torque.Zero;
        Torque momentAboutY = Torque.Zero;
        Torque momentAboutZ = Torque.Zero;
        foreach (LinearLoad linearLoad in this.LinearLoads)
        {
            forceAlongX += linearLoad.Magnitude * linearLoad.NormalizedDirection[0];
            forceAlongY += linearLoad.Magnitude * linearLoad.NormalizedDirection[1];
            forceAlongZ += linearLoad.Magnitude * linearLoad.NormalizedDirection[2];
        }
        //foreach (MomentLoad momentLoad in MomentLoads)
        //{
        //    momentAboutX += momentLoad.Magnitude * momentLoad.
        //}
        return new(forceAlongX, forceAlongY, forceAlongZ, momentAboutX, momentAboutY, momentAboutZ);
    }

    public VectorIdentified GetForceVectorIdentifiedInGlobalCoordinates(
        ForceUnit forceUnit,
        TorqueUnit torqueUnit)
    {
        return new(
            this.Id.GetUnsupportedStructureDisplacementIds().ToList(),
            this.GetForcesInGlobalCoordinates().ToArray(forceUnit, torqueUnit));
    }
}
