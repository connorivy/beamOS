using beamOS.API.Schema.Objects;

namespace beamOS.Tests.Schema.Objects
{
  public partial class AnalyticalModelTests
  {
    //[Theory]
    //[MemberData(nameof(AnalyticalModelTestsData.TestExpandOctreeData), MemberType = typeof(AnalyticalModelTestsData))]
    //public void TestExpandOctree(double[][] nodeLocations, int[][] expectedResults)
    //{
    //  var model = new AnalyticalModel(nodeLocations[0]);

    //  for (var i = 0; i < nodeLocations.Length; i++)
    //    model.AddNode(new Node(nodeLocations[i]), out var _);

    //  //calculate DOFs
    //  var calculated = new int[model.DOFs.Count()][];
    //  for (var i = 0; i < model.DOFs.Count(); i++)
    //  {
    //    calculated[i] = new int[] { model.DOFs.ElementAt(i).NodeId, model.DOFs.ElementAt(i).DofIndex };
    //  }

    //  Assert.Equal(calculated, expectedResults);
    //}
  }
}
