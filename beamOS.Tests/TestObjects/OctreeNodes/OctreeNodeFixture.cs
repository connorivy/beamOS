namespace beamOS.Tests.TestObjects.OctreeNodes;
using beamOS.API.Schema.Objects;
using LanguageExt;

internal class OctreeNodeFixture : SerializableFixtureBase
{
  public OctreeNodeFixture() { }
  public OctreeNodeFixture(OctreeNode node) => this.OctreeNode = node;
  public OctreeNode OctreeNode { get; set; }
  public Option<double[]> ExpectedCenter { get; set; }
  public Option<int> ExpectedNodeDepth { get; set; } = 0;
  public Option<double> ExpectedSize { get; set; }
  public Option<List<Node>> ExpectedNodes { get; set; }
  public Option<List<Element1D>> ExpectedElement1Ds { get; set; }
}
