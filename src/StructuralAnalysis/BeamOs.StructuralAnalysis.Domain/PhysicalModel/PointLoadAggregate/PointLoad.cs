using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.Common.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;

public class PointLoad : BeamOsModelEntity<PointLoadId>
{
    public PointLoad(
        ModelId modelId,
        NodeId nodeId,
        Force force,
        Vector3D direction,
        PointLoadId? id = null
    )
        : base(id, modelId)
    {
        this.ModelId = modelId;
        this.NodeId = nodeId;
        this.Force = force;
        this.Direction = direction;
    }

    public ModelId ModelId { get; private set; }
    public NodeId NodeId { get; private set; }
    public Force Force { get; private set; }
    public Vector3D Direction { get; private set; }

    public Force GetForceInDirection(CoordinateSystemDirection3D direction)
    {
        return direction switch
        {
            CoordinateSystemDirection3D.AlongX => this.Force * this.Direction.X,
            CoordinateSystemDirection3D.AlongY => this.Force * this.Direction.Y,
            CoordinateSystemDirection3D.AlongZ => this.Force * this.Direction.Z,
            CoordinateSystemDirection3D.AboutX
            or CoordinateSystemDirection3D.AboutY
            or CoordinateSystemDirection3D.AboutZ => throw new ArgumentException(
                "Point load has no force about an axis"
            ),
            CoordinateSystemDirection3D.Undefined => throw new ArgumentException(
                "Unexpected value for direction, Undefined"
            ),
            _ => throw new NotImplementedException(),
        };
    }

    public Force GetForceInDirection(Vector3D direction)
    {
        // magnitude of projection of A onto B = (A . B) / | B |
        double magnitudeOfProjection = this.Direction.DotProduct(direction) / direction.Length;
        return this.Force * magnitudeOfProjection;
    }

    [Obsolete("EF Core Constructor")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public PointLoad() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
