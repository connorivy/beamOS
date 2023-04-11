namespace beamOS.Tests.Schema.Objects;

using beamOS.API.Schema.Objects;
using beamOS.Tests.TestObjects.AnalyticalModels;
using beamOS.Tests.TestObjects.AnalyticalModels.OctreeTestObjects;

public partial class AnalyticalModelTests
{
  [Theory]
  [ClassData(typeof(OctreeTestObjects))]
  public void TestExpandOctree(AnalyticalModelFixture fixture)
  {
    var root = fixture.AnalyticalModel.OctreeRoot;
    var calculatedCenter = new double[]
    {
      root.Center.x, root.Center.y, root.Center.z
    };

    _ = fixture.ExpectedOctreeCenter.IfSome(center => Assert.Equal(calculatedCenter, center));
  }
}
