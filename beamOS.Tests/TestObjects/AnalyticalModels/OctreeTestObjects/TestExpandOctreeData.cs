namespace beamOS.Tests.TestObjects.AnalyticalModels.OctreeTestObjects;

using System.Collections.Generic;
using beamOS.API.Schema.Objects;

internal class TestExpandOctreeData : TheoryDataBase<AnalyticalModelFixture>
{
  public override List<AnalyticalModelFixture> AllTestObjects => new()
  {
    ExpandOctreeSimple(),
    ExpandOctreeComplex()
  };

  public static AnalyticalModelFixture ExpandOctreeSimple()
  {
    var am = new AnalyticalModel(minTreeNodeSize: 1, 0, 0, 0);
    am.AddNode(new Node(new double[] { 1, 1, 1 }), out _);

    var fixture = new AnalyticalModelFixture(am)
    {
      ExpectedOctreeCenter = new double[] { .5, .5, .5 }
    };

    return fixture;
  }

  public static AnalyticalModelFixture ExpandOctreeComplex()
  {
    var am = new AnalyticalModel(minTreeNodeSize: 1, 0, 0, 0);

    // center progression

    // 0,0,0 -> .5, .5, -.5 (size 2)
    // -> 1.5, 1.5, -1.5 (size 4)
    // -> 3.5, 3.5, -3.5 (size 8)
    // -> 7.5, 7.5, -7.5 (size 16)

    am.AddNode(new Node(new double[] { 5, 10, -10 }), out _);

    var fixture = new AnalyticalModelFixture(am)
    {
      ExpectedOctreeCenter = new double[] { 7.5, 7.5, -7.5 }
    };

    return fixture;
  }
}
