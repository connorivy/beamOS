using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.PointLoadAggregate;

public class PointLoad : AggregateRoot<PointLoadId>
{
    public PointLoad(NodeId nodeId, Force force, Vector<double> direction, PointLoadId? id = null)
        : base(id ?? new())
    {
        this.NodeId = nodeId;
        this.Force = force;
        this.NormalizedDirection = direction;
    }

    public NodeId NodeId { get; private set; }
    public Force Force { get; private set; }
    public Vector<double> NormalizedDirection { get; private set; }

    public Force GetForceInDirection(CoordinateSystemDirection3D direction)
    {
        return direction switch
        {
            CoordinateSystemDirection3D.AlongX => this.Force * this.NormalizedDirection[0],
            CoordinateSystemDirection3D.AlongY => this.Force * this.NormalizedDirection[1],
            CoordinateSystemDirection3D.AlongZ => this.Force * this.NormalizedDirection[2],
            CoordinateSystemDirection3D.AboutX
            or CoordinateSystemDirection3D.AboutY
            or CoordinateSystemDirection3D.AboutZ
                => throw new ArgumentException("Point load has no force about an axis"),
            CoordinateSystemDirection3D.Undefined
                => throw new ArgumentException("Unexpected value for direction, Undefined"),
            _ => throw new NotImplementedException(),
        };
    }

    public PointLoadData GetData()
    {
        return new(this.Force, this.NormalizedDirection);
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private PointLoad() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
