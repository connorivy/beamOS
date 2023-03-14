using beamOS.API.Schema.Objects;
using static beamOS.API.Schema.Objects.AnalyticalModel;

namespace beamOS.Tests.Schema.Objects
{
  public class AnalyticalModelTests
  {
    //[Theory]
    //[MemberData(nameof(AnalyticalModelTestsData.TestGetGlobalStiffnessMatrixData), MemberType = typeof(AnalyticalModelTestsData))]
    //public void TestGetGlobalStiffnessMatrix(Line baseLine, double rotation, double[,] matrixArray)
    //{
    //}

    [Theory]
    [MemberData(nameof(AnalyticalModelTestsData.TestGetDOFsData), MemberType = typeof(AnalyticalModelTestsData))]
    public void TestGetDOFs(double[][] nodeLocations, bool[][] nodeFixities, int[][] expectedResults)
    {
      var model = new AnalyticalModel(nodeLocations[0]);

      for (var i = 0; i < nodeLocations.Length; i++)
        model.AddNode(new Node(nodeLocations[i], nodeFixities[i]), out var _);

      //calculate DOFs
      var calculated = new int[model.DOFs.Count()][];
      for (var i = 0; i < model.DOFs.Count(); i++)
      {
        calculated[i] = new int[] { model.DOFs.ElementAt(i).NodeId, model.DOFs.ElementAt(i).DofIndex };
      }

      Assert.Equal(calculated, expectedResults);
    }

    [Theory]
    [MemberData(nameof(AnalyticalModelTestsData.TestUnlockModelData), MemberType = typeof(AnalyticalModelTestsData))]
    public void TestUnlockModel(double E, double G, double A, double Iz, double Iy, double J, double rotation, double[][] P0, double[][] P1)
    {
      Assert.Equal(P0.Length, P1.Length);

      var model = new AnalyticalModel(P0[0]);
      var material = new Material() { E = E, G = G };
      var section = new SectionProfile() { A = A, Iz = Iz, Iy = Iy, J = J };

      for (var i = 0; i < P0.Length; i++)
      {
        var el1D = new Element1D(new Line(P0[i], P1[i]), section, material);
        var _ = el1D.LocalStiffnessMatrix;
        model.AddElement1D(el1D);
        Assert.NotNull(el1D._localStiffnessMatrix);
      }

      _ = model.DOFs;
      Assert.NotNull(model._dofs);

      // clear analysis results aka "unlock" model
      model.UnlockObject(model);
      Assert.Null(model._dofs);
      foreach (var el in model.Element1Ds.Values)
      {
        Assert.Null(el._localStiffnessMatrix);
      }
    }
  }
}
