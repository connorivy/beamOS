using BeamOs.Domain.Common.ValueObjects;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public class NodeFixture(Point locationPoint, Restraint restraint, Guid modelId) : FixtureBase
{
    public Point LocationPoint { get; } = locationPoint;
    public Restraint Restraint { get; } = restraint;
    public override GuidWrapperForModelId ModelId { get; } = new(modelId);
}

public record NodeResultFixture(NodeFixture Node, Forces Forces, Displacements Displacements);
