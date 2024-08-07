using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.MomentLoadAggregate;

public class MomentLoad : AggregateRoot<MomentLoadId>
{
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
        this.AxisDirection = axisDirection.Normalize(2);
    }

    public ModelId ModelId { get; set; }
    public NodeId NodeId { get; set; }
    public Torque Torque { get; set; }
    public Vector<double> AxisDirection { get; set; }

    public Torque GetTorqueInDirection(CoordinateSystemDirection3D direction)
    {
        return direction switch
        {
            CoordinateSystemDirection3D.AlongX
            or CoordinateSystemDirection3D.AlongY
            or CoordinateSystemDirection3D.AlongZ
                => throw new ArgumentException("Moment load has no torque along an axis"),
            CoordinateSystemDirection3D.AboutX => this.Torque * this.AxisDirection[0],
            CoordinateSystemDirection3D.AboutY => this.Torque * this.AxisDirection[1],
            CoordinateSystemDirection3D.AboutZ => this.Torque * this.AxisDirection[2],
            CoordinateSystemDirection3D.Undefined
                => throw new ArgumentException("Unexpected value for direction, Undefined"),
            _ => throw new NotImplementedException(),
        };
    }

    public Torque GetForceAboutAxis(Vector3D direction)
    {
        // magnitude of projection of A onto B = (A . B) / | B |
        double magnitudeOfProjection =
            new Vector3D(
                this.AxisDirection[0],
                this.AxisDirection[1],
                this.AxisDirection[2]
            ).DotProduct(direction) / direction.Length;
        return this.Torque * magnitudeOfProjection;
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private MomentLoad() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
