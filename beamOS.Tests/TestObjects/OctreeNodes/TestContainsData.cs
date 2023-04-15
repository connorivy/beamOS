namespace beamOS.Tests.TestObjects.OctreeNodes;
using System.Collections.Generic;
using beamOS.API.Schema.Objects;

internal class TestExpandOctreeData : TheoryDataBase<OctreeNodeFixture>
{
  public override List<OctreeNodeFixture> AllTestObjects => new()
  {
    //Contains1(),
    //ExpandOctreeComplex()
  };

  //public static OctreeNodeFixture Contains1()
  //{
  //  var am = new AnalyticalModel(minTreeNodeSize: 1, 0, 0, 0);
  //  am.AddNode(new Node(new double[] { 1, 1, 1 }), out _);

  //  var fixture = new OctreeNodeFixture(am)
  //  {
  //    ExpectedOctreeCenter = new double[] { .5, .5, .5 }
  //  };

  //  return fixture;
  //}
}
