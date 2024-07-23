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

public record NodeDisplacementResultFixture
{
    public required NodeFixture2 NodeFixture { get; init; }
    public Length? DisplacementAlongX { get; init; }
    public Length? DisplacementAlongY { get; init; }
    public Length? DisplacementAlongZ { get; init; }
    public Angle? RotationAboutX { get; init; }
    public Angle? RotationAboutY { get; init; }
    public Angle? RotationAboutZ { get; init; }
}

public record NodeForceResultFixture
{
    public required NodeFixture2 NodeFixture { get; init; }
    public Length? ForceAlongX { get; init; }
    public Length? ForceAlongY { get; init; }
    public Length? ForceAlongZ { get; init; }
    public Angle? TorqueAboutX { get; init; }
    public Angle? TorqueAboutY { get; init; }
    public Angle? TorqueAboutZ { get; init; }
}
