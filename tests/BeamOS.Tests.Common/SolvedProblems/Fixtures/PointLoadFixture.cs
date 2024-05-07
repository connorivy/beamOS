using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public class PointLoadFixture(NodeFixture node, Force force, UnitVector3D normalizedDirection)
    : FixtureBase
{
    public NodeFixture Node { get; } = node;
    public Force Force { get; } = force;
    public UnitVector3D NormalizedDirection { get; } = normalizedDirection;
}
