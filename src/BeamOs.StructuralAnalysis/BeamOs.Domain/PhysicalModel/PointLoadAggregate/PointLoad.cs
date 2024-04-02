using BeamOs.Domain.Common.Models;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.PointLoadAggregate;

public class PointLoad : AggregateRoot<PointLoadId>
{
    public PointLoad(NodeId nodeId, ImmutablePointLoad immutablePointLoad, PointLoadId? id = null)
        : base(id ?? new())
    {
        this.NodeId = nodeId;
        this.Data = immutablePointLoad;
    }

    public PointLoad(NodeId nodeId, Force force, Vector<double> direction, PointLoadId? id = null)
        : this(nodeId, new ImmutablePointLoad(force, direction.Normalize(2)), id) { }

    public NodeId NodeId { get; private set; }

    //public Force Force { get; private set; }
    //public Vector<double> NormalizedDirection { get; private set; }
    public ImmutablePointLoad Data { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private PointLoad() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
