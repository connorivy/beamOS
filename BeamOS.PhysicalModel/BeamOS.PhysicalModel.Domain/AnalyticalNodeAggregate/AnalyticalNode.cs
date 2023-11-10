using BeamOS.Common.Domain.Models;
using BeamOS.Common.Domain.ValueObjects;
using BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate;
public class AnalyticalNode : AggregateRoot<AnalyticalNodeId>
{
    public AnalyticalModelId ModelId { get; }
    public Point LocationPoint { get; private set; }
    public AnalyticalNode(
        AnalyticalModelId modelId,
        double xCoordinate,
        double yCoordinate,
        double zCoordinate,
        LengthUnit lengthUnit,
        Restraints? restraint = null,
        AnalyticalNodeId? id = null) : base(id ?? new())
    {
        this.ModelId = modelId;
        this.LocationPoint = new(xCoordinate, yCoordinate, zCoordinate, lengthUnit);
        this.Restraints = restraint ?? Restraints.Free;
    }

    public AnalyticalNode(
        AnalyticalModelId modelId,
        Length xCoordinate,
        Length yCoordinate,
        Length zCoordinate,
        Restraints? restraint = null,
        AnalyticalNodeId? id = null) : base(id ?? new())
    {
        this.ModelId = modelId;
        this.LocationPoint = new(xCoordinate, yCoordinate, zCoordinate);
        this.Restraints = restraint ?? Restraints.Free;
    }

    public List<PointLoadId> PointLoadIds { get; } = [];
    //public List<MomentLoad> MomentLoads { get; } = [];
    public Restraints Restraints { get; set; }
}
