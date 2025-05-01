using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinationAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;

public class MomentLoad : BeamOsModelEntity<MomentLoadId>
{
    public MomentLoad(
        ModelId modelId,
        NodeId nodeId,
        LoadCaseId loadCaseId,
        Torque torque,
        Vector3D axisDirection,
        MomentLoadId? id = null
    )
        : base(id ?? new(), modelId)
    {
        this.NodeId = nodeId;
        this.LoadCaseId = loadCaseId;
        this.Torque = torque;
        this.AxisDirection = new(axisDirection.Normalize());
    }

    public NodeId NodeId { get; set; }
    public LoadCaseId LoadCaseId { get; set; }
    public Torque Torque { get; set; }
    public UnitVector3d AxisDirection { get; set; }

    public Torque GetTorqueInDirection(CoordinateSystemDirection3D direction)
    {
        return direction switch
        {
            CoordinateSystemDirection3D.AlongX
            or CoordinateSystemDirection3D.AlongY
            or CoordinateSystemDirection3D.AlongZ => throw new ArgumentException(
                "Moment load has no torque along an axis"
            ),
            CoordinateSystemDirection3D.AboutX => this.Torque * this.AxisDirection.X,
            CoordinateSystemDirection3D.AboutY => this.Torque * this.AxisDirection.Y,
            CoordinateSystemDirection3D.AboutZ => this.Torque * this.AxisDirection.Z,
            CoordinateSystemDirection3D.Undefined => throw new ArgumentException(
                "Unexpected value for direction, Undefined"
            ),
            _ => throw new NotImplementedException(),
        };
    }

    public Torque GetForceAboutAxis(Vector3D direction)
    {
        // magnitude of projection of A onto B = (A . B) / | B |
        double magnitudeOfProjection =
            new Vector3D(
                this.AxisDirection.X,
                this.AxisDirection.Y,
                this.AxisDirection.Z
            ).DotProduct(direction) / direction.Length;
        return this.Torque * magnitudeOfProjection;
    }

    public Torque GetScaledTorque(
        CoordinateSystemDirection3D direction,
        LoadCombination loadCombination
    ) => this.GetTorqueInDirection(direction) * loadCombination.GetFactor(this.LoadCaseId);

    public double GetScaledTorque(
        CoordinateSystemDirection3D direction,
        TorqueUnit torqueUnit,
        LoadCombination loadCombination
    ) => this.GetScaledTorque(direction, loadCombination).As(torqueUnit);

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private MomentLoad() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
