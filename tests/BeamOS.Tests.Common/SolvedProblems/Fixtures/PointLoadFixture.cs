using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public class PointLoadFixture(NodeFixture node, Force force, Vector3D direction) : FixtureBase
{
    public NodeFixture Node { get; } = node;
    public Force Force { get; } = force;
    public Vector3D Direction { get; } = direction;
}
