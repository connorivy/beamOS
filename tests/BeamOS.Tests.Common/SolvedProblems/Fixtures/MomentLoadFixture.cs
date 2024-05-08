using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public class MomentLoadFixture(
    NodeFixture node,
    Torque torque,
    Vector3D normalizedAxisDirection
) : FixtureBase
{
    public NodeFixture Node { get; } = node;
    public Torque Torque { get; } = torque;
    public Vector3D NormalizedAxisDirection { get; } = normalizedAxisDirection;
}
