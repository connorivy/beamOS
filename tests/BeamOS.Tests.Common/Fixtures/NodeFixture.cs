using BeamOs.ApiClient.Builders;
using BeamOs.Domain.Common.ValueObjects;
using UnitsNet;

namespace BeamOS.Tests.Common.Fixtures;

public class NodeFixture2 : FixtureBase2
{
    public required Guid ModelId { get; init; }
    public required Point LocationPoint { get; init; }
    public required Restraint Restraint { get; init; }
    public ICollection<PointLoadFixture2> PointLoads { get; init; } = [];
    public ICollection<MomentLoadFixture2> MomentLoads { get; init; } = [];
}

public record NodeResultFixture2(NodeFixture2 Node, Forces Forces, Displacements Displacements);

public record NodeResultFixture
{
    public required FixtureId NodeId { get; init; }

    public Length? DisplacementAlongX { get; init; }
    public Length? DisplacementAlongY { get; init; }
    public Length? DisplacementAlongZ { get; init; }
    public Length LengthTolerance { get; init; } = new(.3, UnitsNet.Units.LengthUnit.Inch);

    public Angle? RotationAboutX { get; init; }
    public Angle? RotationAboutY { get; init; }
    public Angle? RotationAboutZ { get; init; }
    public Angle AngleTolerance { get; init; } = new(1, UnitsNet.Units.AngleUnit.Degree);

    public Force? ForceAlongX { get; init; }
    public Force? ForceAlongY { get; init; }
    public Force? ForceAlongZ { get; init; }
    public Force ForceTolerance { get; init; } = new(.1, UnitsNet.Units.ForceUnit.KilopoundForce);

    public Torque? TorqueAboutX { get; init; }
    public Torque? TorqueAboutY { get; init; }
    public Torque? TorqueAboutZ { get; init; }
    public Torque TorqueTolerance { get; init; } =
        new(5, UnitsNet.Units.TorqueUnit.KilopoundForceInch);
}

public record NodeForceResultFixture
{
    public required NodeFixture2 NodeFixture { get; init; }
    public Force? ForceAlongX { get; init; }
    public Force? ForceAlongY { get; init; }
    public Force? ForceAlongZ { get; init; }
    public Torque? TorqueAboutX { get; init; }
    public Torque? TorqueAboutY { get; init; }
    public Torque? TorqueAboutZ { get; init; }
}
