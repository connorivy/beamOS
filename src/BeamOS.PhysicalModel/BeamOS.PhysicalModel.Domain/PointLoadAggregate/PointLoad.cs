using BeamOS.Common.Domain.Models;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOS.PhysicalModel.Domain.PointLoadAggregate;
public class PointLoad : AggregateRoot<PointLoadId>
{
    public PointLoad(
        NodeId nodeId,
        Force force,
        Vector<double> direction,
        PointLoadId? id = null) : base(id ?? new())
    {
        this.NodeId = nodeId;
        this.Force = force;
        this.NormalizedDirection = direction.Normalize(2);
    }

    public NodeId NodeId { get; private set; }
    public Force Force { get; private set; }
    public Vector<double> NormalizedDirection { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private PointLoad() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
