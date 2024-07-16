using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOS.Tests.Common.Fixtures;

public class MomentLoadFixture(NodeFixture node, Torque torque, Vector3D axisDirection)
    : FixtureBase
{
    public NodeFixture Node { get; } = node;
    public Torque Torque { get; } = torque;
    public Vector3D AxisDirection { get; } = axisDirection;
}

public class MomentLoadFixture2 : FixtureBase2
{
    public required Guid ModelId { get; init; }
    public required Torque Torque { get; init; }
    public required Vector3D AxisDirection { get; init; }
    public required Lazy<NodeFixture2> Node { get; init; }
    public Guid NodeId => this.Node.Value.Id;
}