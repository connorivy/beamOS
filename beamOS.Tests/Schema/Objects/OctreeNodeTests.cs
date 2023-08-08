namespace beamOS.Tests.Schema.Objects;

using beamOS.API.Schema.Objects;
using beamOS.Tests.TestObjects.OctreeNodes;
using global::Objects.Geometry;

public class OctreeNodeTests
{
  [Theory]
  [InlineData(new double[3] { 0, 0, 0 }, 10, new double[3] { 5, 5, 5 }, true)] // upper bound is inclusive
  [InlineData(new double[3] { 0, 0, 0 }, 10, new double[3] { -5, -5, -5 }, false)] // lower bound is exclusive
  [InlineData(new double[3] { 0, 0, 0 }, 10, new double[3] { -4.999, -4.999, -4.999 }, true)]
  [InlineData(new double[3] { 0, 0, 0 }, 10, new double[3] { 5, 5, -5 }, false)]
  [InlineData(new double[3] { 0, 0, 0 }, 10, new double[3] { 5, 5, -4.999 }, true)]
  [InlineData(new double[3] { 0, 0, 0 }, 10, new double[3] { 5, -5, -5 }, false)]
  [InlineData(new double[3] { 0, 0, 0 }, 10, new double[3] { -5, 5, -5 }, false)]
  [InlineData(new double[3] { 0, 0, 0 }, 10, new double[3] { -5, -5, 5 }, false)]
  public void TestContains(double[] center, double size, double[] point, bool expectedValue)
  {
    var octreeNode = new OctreeNode(ModelSettings.Dummy, center, size, null);

    var contains = octreeNode.Contains(new Point(point[0], point[1], point[2]));

    Assert.Equal(expectedValue, contains);
  }

  [Theory]
  [ClassData(typeof(TestCurveIntersectsTheoryData))]
  public void TestCurveIntersects(TestCurveIntersectsFixture fixture)
  {
    var octreeNode = new OctreeNode(ModelSettings.Dummy, fixture.Center, fixture.Size, null);

    var contains = octreeNode.CurveIntersects(fixture.Curve);

    Assert.Equal(fixture.ExpectedValue, contains);
  }
}