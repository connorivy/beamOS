using BeamOs.Domain.Common.ValueObjects;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public class NodeFixture(Point locationPoint, Restraint restraint) : FixtureBase
{
    public Point LocationPoint { get; } = locationPoint;
    public Restraint Restraint { get; } = restraint;
}

public record NodeResultFixture(NodeFixture Node, Forces Forces, Displacements Displacements);
