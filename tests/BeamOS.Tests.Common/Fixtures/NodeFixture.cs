using BeamOs.Domain.Common.ValueObjects;

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
