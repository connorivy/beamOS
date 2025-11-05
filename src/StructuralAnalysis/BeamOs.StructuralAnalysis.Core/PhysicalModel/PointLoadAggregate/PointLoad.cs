using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using MathNet.Spatial.Euclidean;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;

internal class PointLoad
    : BeamOsModelEntity<PointLoadId>,
        IBeamOsModelEntity<PointLoadId, PointLoad>
{
    public PointLoad(
        ModelId modelId,
        NodeId nodeId,
        LoadCaseId loadCaseId,
        Force force,
        Vector3D direction,
        PointLoadId? id = null
    )
        : base(id ?? new(), modelId)
    {
        this.NodeId = nodeId;
        this.LoadCaseId = loadCaseId;
        this.Force = force;
        this.Direction = direction;
    }

    public NodeId NodeId { get; private set; }
    public LoadCaseId LoadCaseId { get; private set; }
    public LoadCase? LoadCase { get; private set; }
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

    public double GetScaledForce(
        CoordinateSystemDirection3D direction,
        ForceUnit forceUnit,
        LoadCombination loadCombination
    ) => this.GetScaledForce(direction, loadCombination).As(forceUnit);

    public Force GetScaledForce(
        CoordinateSystemDirection3D direction,
        LoadCombination loadCombination
    ) => this.GetForceInDirection(direction) * loadCombination.GetFactor(this.LoadCaseId);

    public bool MemberwiseEquals(PointLoad other)
    {
        return this.NodeId == other.NodeId
            && this.LoadCaseId == other.LoadCaseId
            && this.Force.Equals(other.Force, new Force(.01, ForceUnit.Newton))
            && this.Direction.Equals(other.Direction);
    }

    [Obsolete("EF Core Constructor")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public PointLoad() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
