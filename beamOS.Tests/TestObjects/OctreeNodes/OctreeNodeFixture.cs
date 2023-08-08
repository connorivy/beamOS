namespace beamOS.Tests.TestObjects.OctreeNodes;
using beamOS.API.Schema.Objects;

internal class OctreeNodeFixture : SerializableFixtureBase
{
  public OctreeNodeFixture() { }
  public OctreeNodeFixture(OctreeNode node) => this.OctreeNode = node;
  public OctreeNode OctreeNode { get; set; }
  public double[]? ExpectedCenter { get; set; }
  public int? ExpectedNodeDepth { get; set; } = 0;
  public double? ExpectedSize { get; set; }
  public List<Node>? ExpectedNodes { get; set; }
  public List<Element1D>? ExpectedElement1Ds { get; set; }
}
