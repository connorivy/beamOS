using BeamOS.Common.Domain.Models;
using BeamOS.Common.Domain.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.Common.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.NodeAggregate.ValueObjects;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.DirectStiffnessMethod.Domain.NodeAggregate;
public class Node : AggregateRoot<NodeId>
{
    public Point LocationPoint { get; private set; }
    private Node(
        NodeId id,
        double xCoordinate,
        double yCoordinate,
        double zCoordinate,
        LengthUnit lengthUnit,
        Restraints? restraint = null) : base(id)
    {
        this.LocationPoint = new(xCoordinate, yCoordinate, zCoordinate, lengthUnit);
        this.Restraints = restraint ?? Restraints.Free;
    }
    public static Node Create(
        double xCoordinate,
        double yCoordinate,
        double zCoordinate,
        LengthUnit lengthUnit,
        Restraints? restraint = null
    )
    {
        return new(NodeId.CreateUnique(), xCoordinate, yCoordinate, zCoordinate, lengthUnit, restraint);
    }
    private Node(
        NodeId id,
        Length xCoordinate,
        Length yCoordinate,
        Length zCoordinate,
        Restraints? restraint = null) : base(id)
    {
        this.LocationPoint = new(xCoordinate, yCoordinate, zCoordinate);
        this.Restraints = restraint ?? Restraints.Free;
    }
    public static Node Create(
        Length xCoordinate,
        Length yCoordinate,
        Length zCoordinate,
        Restraints? restraint = null
    )
    {
        return new(NodeId.CreateUnique(), xCoordinate, yCoordinate, zCoordinate, restraint);
    }

    public List<LinearLoad> LinearLoads { get; } = [];
    public List<MomentLoad> MomentLoads { get; } = [];
    public Restraints Restraints { get; set; }
    public Forces? Forces { get; set; }

    public Forces GetForcesInGlobalCoordinates()
    {
        var forceAlongX = Force.Zero;
        var forceAlongY = Force.Zero;
        var forceAlongZ = Force.Zero;
        var momentAboutX = Torque.Zero;
        var momentAboutY = Torque.Zero;
        var momentAboutZ = Torque.Zero;
        foreach (var linearLoad in this.LinearLoads)
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
