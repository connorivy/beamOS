using BeamOS.Common.Domain.Models;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOS.PhysicalModel.Domain.MomentLoadAggregate;

public class MomentLoad : AggregateRoot<MomentLoadId>
{
    public MomentLoad(
        NodeId nodeId,
        Torque torque,
        Vector<double> axisDirection,
        MomentLoadId? id = null
    )
        : base(id ?? new())
    {
        this.NodeId = nodeId;
        this.Torque = torque;
        this.NormalizedAxisDirection = axisDirection.Normalize(2);
    }

    public NodeId NodeId { get; private set; }
    public Torque Torque { get; private set; }
    public Vector<double> NormalizedAxisDirection { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private MomentLoad() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
