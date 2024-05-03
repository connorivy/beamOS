using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.MomentLoadAggregate;

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

    public ModelId ModelId { get; private set; }

    public MomentLoad(
        ModelId modelId,
        NodeId nodeId,
        Torque torque,
        Vector<double> axisDirection,
        MomentLoadId? id = null
    )
        : base(id ?? new())
    {
        this.ModelId = modelId;
        this.NodeId = nodeId;
        this.Torque = torque;
        this.NormalizedAxisDirection = axisDirection.Normalize(2);
    }

    public NodeId NodeId { get; private set; }
    public Torque Torque { get; private set; }
    public Vector<double> NormalizedAxisDirection { get; private set; }

    public Torque GetTorqueInDirection(CoordinateSystemDirection3D direction)
    {
        return direction switch
        {
            CoordinateSystemDirection3D.AlongX
            or CoordinateSystemDirection3D.AlongY
            or CoordinateSystemDirection3D.AlongZ
                => throw new ArgumentException("Moment load has no torque along an axis"),
            CoordinateSystemDirection3D.AboutX => this.Torque * this.NormalizedAxisDirection[0],
            CoordinateSystemDirection3D.AboutY => this.Torque * this.NormalizedAxisDirection[1],
            CoordinateSystemDirection3D.AboutZ => this.Torque * this.NormalizedAxisDirection[2],
            CoordinateSystemDirection3D.Undefined
                => throw new ArgumentException("Unexpected value for direction, Undefined"),
            _ => throw new NotImplementedException(),
        };
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private MomentLoad() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
