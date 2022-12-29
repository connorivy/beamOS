using beamOS.API;
using beamOS.API.Schema.Objects;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
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

    [Theory]
    [MemberData(nameof(Element1DTestsData.TestGetRotationMatrixData), MemberType = typeof(Element1DTestsData))]
    [MemberData(nameof(Element1DTestsData.TestGetRotationMatrixData_VerticalMembers), MemberType = typeof(Element1DTestsData))]
    [MemberData(nameof(Element1DTestsData.TestGetRotationMatrixData_Misaligned), MemberType = typeof(Element1DTestsData))]
    public void TestGetRotationMatrix(Line baseLine, double rotation, double[,] matrixArray)
    {
      var element1D = new Element1D(baseLine, null, null);
      element1D.ProfileRotation = rotation;

      var rotationMatrix = element1D.GetRotationMatrix(baseLine);
      var testMatrix = DenseMatrix.OfArray(matrixArray);

      var equal = rotationMatrix.AlmostEqual(testMatrix, .0001);
      if (!equal)
      {
        System.Diagnostics.Debug.WriteLine(rotationMatrix.ToString());
        System.Diagnostics.Debug.WriteLine(testMatrix.ToString());
      }

      Assert.True(equal);
    }
  }
}
