using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public class MomentLoadFixture(
    NodeFixture node,
    Torque torque,
    UnitVector3D normalizedAxisDirection
) : FixtureBase
{
    public NodeFixture Node { get; } = node;
    public Torque Torque { get; } = torque;
    public UnitVector3D NormalizedAxisDirection { get; } = normalizedAxisDirection;
}
