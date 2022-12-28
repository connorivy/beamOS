using beamOS.API;
using beamOS.API.Schema.Objects;
using MathNet.Numerics.LinearAlgebra.Double;

namespace beamOS.Tests.Schema.Objects
{
  public class Element1DTests
  {
    Element1D Element1D;
    public Element1DTests()
    {
      var curve = new Line(new double[] {0, 0, 0}, new double[] {10, 0, 0});
      Element1D = new Element1D(curve, null, null, ElementType.Beam);
    }

    #region inLineData
    public static List<object[]> TestGetRotationMatrixEnumerator => new List<object[]>
    {
      new object[]
      {
        new[] {0.0,0,0},
        new[] {10.0,0,0},
        0,
        new[,]
        {
          { 1.0, 0, 0},
          { 0, 1.0, 0},
          { 0, 0, 1.0},
        }
      },
    };

    [Theory]
    [MemberData(nameof(TestGetRotationMatrixEnumerator))]
    #endregion
    public void TestGetRotationMatrix(double[] p0, double[] p1, double profileRotation, double[,] matrixArray)
    {
      var curve = new Line(p0, p1);
      var element1D = new Element1D(curve, null, null);
      element1D.ProfileRotation = profileRotation;

      var testMatrix = DenseMatrix.OfArray(matrixArray);
      var rotationMatrix = element1D.GetRotationMatrix(curve);

      Assert.Equal(testMatrix, rotationMatrix);
    }
  }
}
