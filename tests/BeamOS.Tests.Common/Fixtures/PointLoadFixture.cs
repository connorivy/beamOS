using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOS.Tests.Common.Fixtures;

public class PointLoadFixture(NodeFixture node, Force force, Vector3D direction) : FixtureBase
{
    public NodeFixture Node { get; } = node;
    public Force Force { get; } = force;
    public Vector3D Direction { get; } = direction;
}

public class PointLoadFixture2 : FixtureBase2
{
    public required Guid ModelId { get; init; }
    public required Force Force { get; init; }
    public required Vector3D Direction { get; init; }
    public required Lazy<NodeFixture2> Node { get; init; }
    public Guid NodeId => this.Node.Value.Id;
}
