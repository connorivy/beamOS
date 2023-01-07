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
    public void TestGetRotationMatrix(Line baseLine, double rotation, double[,] matrixArray)
    {
      var element1D = new Element1D(baseLine, null, null);
      element1D.ProfileRotation = rotation;

      var rotationMatrix = element1D.GetRotationMatrix(baseLine);
      var testMatrix = DenseMatrix.OfArray(matrixArray);

      var equal = rotationMatrix.AlmostEqual(testMatrix, .0001);
      //if (!equal)
      //{
      //  System.Diagnostics.Debug.WriteLine(rotationMatrix.ToString());
      //  System.Diagnostics.Debug.WriteLine(testMatrix.ToString());
      //}

      Assert.True(equal);
    }

    [Theory]
    [MemberData(nameof(Element1DTestsData.TestGetTransformationMatrixData), MemberType = typeof(Element1DTestsData))]
    public void TestGetTransformationMatrix(double[,] rMatrixArray, bool exceptionThrown = false)
    {
      var element1D = new Element1D(null, null, null);
      var rMatrix = DenseMatrix.OfArray(rMatrixArray);
      if (exceptionThrown)
      {
        Assert.ThrowsAny<Exception>(() => element1D.GetTransformationMatrix(rMatrix));
        return;
      }

      var tMatrix = element1D.GetTransformationMatrix(rMatrix);

      // test non empty parts of the matrix
      var subMatrix = tMatrix.SubMatrix(0, 3, 0, 3);
      Assert.True(subMatrix.AlmostEqual(rMatrix, .0001));
      subMatrix = tMatrix.SubMatrix(3, 3, 3, 3);
      Assert.True(subMatrix.AlmostEqual(rMatrix, .0001));
      subMatrix = tMatrix.SubMatrix(6, 3, 6, 3);
      Assert.True(subMatrix.AlmostEqual(rMatrix, .0001));
      subMatrix = tMatrix.SubMatrix(9, 3, 9, 3);
      Assert.True(subMatrix.AlmostEqual(rMatrix, .0001));

      // test empty parts

      // 9x3 submatrix on the left and right side 
      subMatrix = Matrix<double>.Build.Dense(9, 3);
      var subMatrix2 = tMatrix.SubMatrix(3, 9, 0, 3);
      Assert.True(subMatrix.Equals(subMatrix2));
      subMatrix2 = tMatrix.SubMatrix(0, 9, 9, 3);
      Assert.True(subMatrix.Equals(subMatrix2));

      // 6x3 submatricies
      subMatrix = Matrix<double>.Build.Dense(6, 3);
      subMatrix2 = tMatrix.SubMatrix(6, 6, 3, 3);
      Assert.True(subMatrix.Equals(subMatrix2));
      subMatrix2 = tMatrix.SubMatrix(0, 6, 6, 3);
      Assert.True(subMatrix.Equals(subMatrix2));

      // 3x3 submatricies
      subMatrix = Matrix<double>.Build.Dense(3, 3);
      subMatrix2 = tMatrix.SubMatrix(0, 3, 3, 3);
      Assert.True(subMatrix.Equals(subMatrix2));
      subMatrix2 = tMatrix.SubMatrix(9, 3, 6, 3);
      Assert.True(subMatrix.Equals(subMatrix2));
    }
  }
}
