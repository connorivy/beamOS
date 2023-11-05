using BeamOS.Common.Domain.Models;
using BeamOS.Common.Domain.ValueObjects;
using BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate;
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

    public List<PointLoadId> LinearLoads { get; } = [];
    //public List<MomentLoad> MomentLoads { get; } = [];
    public Restraints Restraints { get; set; }
}
