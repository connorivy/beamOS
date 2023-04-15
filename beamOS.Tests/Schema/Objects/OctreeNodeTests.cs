namespace beamOS.Tests.Schema.Objects;

using beamOS.API.Schema.Objects;
using beamOS.Tests.TestObjects.AnalyticalModels.OctreeTestObjects;
using global::Objects.Geometry;
using LanguageExt;

public class OctreeNodeTests
{
  //[Theory]
  //[ClassData(typeof(TestExpandOctreeData))]
  //public void TestSetTreeNode(AnalyticalModelFixture fixture)
  //{
  //  var root = fixture.AnalyticalModel.OctreeRoot;
  //  var calculatedCenter = new double[]
  //  {
  //    root.Center.x, root.Center.y, root.Center.z
  //  };

  //  _ = fixture.ExpectedOctreeCenter.IfSome(center => Assert.Equal(calculatedCenter, center));
  //}

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
    var octreeNode = new OctreeNode(center, size, Option<OctreeNode>.None);

    var contains = octreeNode.Contains(new Point(point[0], point[1], point[2]));

    Assert.Equal(expectedValue, contains);
  }
}
