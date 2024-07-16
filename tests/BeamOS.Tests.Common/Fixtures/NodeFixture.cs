using BeamOs.Domain.Common.ValueObjects;

namespace BeamOS.Tests.Common.Fixtures;

public class NodeFixture(Point locationPoint, Restraint restraint, Guid modelId) : FixtureBase
{
    public Point LocationPoint { get; } = locationPoint;
    public Restraint Restraint { get; } = restraint;
    public override GuidWrapperForModelId ModelId { get; } = new(modelId);
}

public record NodeResultFixture(NodeFixture Node, Forces Forces, Displacements Displacements);

public record NodeResultFixture2(NodeFixture2 Node, Forces Forces, Displacements Displacements);

public class NodeFixture2 : FixtureBase2
{
    public required Guid ModelId { get; init; }
    public required Point LocationPoint { get; init; }
    public required Restraint Restraint { get; init; }

    //public ICollection<Func<Guid, PointLoadFixture2>> PointLoadFixtures { get; init; } = [];
    public ICollection<PointLoadFixture2> PointLoads { get; init; } = [];
    public ICollection<MomentLoadFixture2> MomentLoads { get; init; } = [];

    //public NodeFixture2()
    //{
    //    this.PointLoads = PointLoadFixtures.Select(func => func(this.Id)).ToList();
    //}
}
