using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.PointLoadAggregate;

public class PointLoad : AggregateRoot<PointLoadId>
{
    public PointLoad(
        ModelId modelId,
        NodeId nodeId,
        Force force,
        Vector3D direction,
        PointLoadId? id = null
    )
        : base(id ?? new())
    {
        this.ModelId = modelId;
        this.NodeId = nodeId;
        this.Force = force;
        this.NormalizedDirection = direction;
    }

    public ModelId ModelId { get; private set; }
    public NodeId NodeId { get; private set; }
    public Force Force { get; private set; }
    public Vector3D NormalizedDirection { get; private set; }

    public Force GetForceInDirection(CoordinateSystemDirection3D direction)
    {
        return direction switch
        {
            CoordinateSystemDirection3D.AlongX => this.Force * this.NormalizedDirection.X,
            CoordinateSystemDirection3D.AlongY => this.Force * this.NormalizedDirection.Y,
            CoordinateSystemDirection3D.AlongZ => this.Force * this.NormalizedDirection.Z,
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

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private PointLoad() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
