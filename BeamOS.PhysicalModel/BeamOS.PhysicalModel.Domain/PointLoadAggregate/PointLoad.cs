using BeamOS.Common.Domain.Models;
using BeamOS.PhysicalModel.Domain.Common.ValueObjects;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOS.PhysicalModel.Domain.PointLoadAggregate;
public class PointLoad : AggregateRoot<PointLoadId>
{
    private PointLoad(PointLoadId id) : base(id) { }
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

    public NodeId NodeId { get; set; }
    public Force Force { get; set; }
    public Vector<double> NormalizedDirection { get; set; }
}
