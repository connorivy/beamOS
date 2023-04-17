namespace beamOS.Tests.Schema.Objects;

using beamOS.API.Schema.Objects;
using beamOS.Tests.TestObjects;
using beamOS.Tests.TestObjects.Element1Ds;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Xunit;

public class Element1DTests
{
  [Theory]
  [MemberData(nameof(Element1DTestsData.TestGetRotationMatrixData), MemberType = typeof(Element1DTestsData))]
  public void TestGetRotationMatrix(Line baseLine, double rotation, double[,] matrixArray)
  {
    var element1D = new Element1D(baseLine, null, null)
    {
      ProfileRotation = rotation
    };

    var rotationMatrix = element1D.GetRotationMatrix();
    var testMatrix = DenseMatrix.OfArray(matrixArray);

    var equal = rotationMatrix.AlmostEqual(testMatrix, .0001);

    Assert.True(equal);
  }

  [SkippableTheory]
  [ClassData(typeof(AllElement1DFixtures))]
  public void TestGetRotationMatrix2(Element1DFixture fixture)
  {
    var rotationMatrix = fixture.Element.GetRotationMatrix();

    _ = fixture.ExpectedRotationMatrix.Match(
      m => rotationMatrix.AssertAlmostEqual(m),
      () => throw new SkipException("Insufficient expected data present on test fixture")
    );
  }

  [Theory]
  [MemberData(nameof(Element1DTestsData.TestGetTransformationMatrixData), MemberType = typeof(Element1DTestsData))]
  public void TestGetTransformationMatrix(double[,] rMatrixArray, bool exceptionThrown = false)
  {
    var element1D = new Element1D(null, null, null);
    var rMatrix = DenseMatrix.OfArray(rMatrixArray);
    if (exceptionThrown)
    {
      _ = Assert.ThrowsAny<Exception>(() => element1D.GetTransformationMatrix(rMatrix));
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

  [Theory]
  [ClassData(typeof(AllElement1DFixtures))]
  public void TestGetTransformationMatrix2(Element1DFixture fixture)
  {
    var transformationMatrix = fixture.Element.GetTransformationMatrix(fixture.Element.GetRotationMatrix());

    _ = fixture.ExpectedTransformationMatrix.IfSome(m => transformationMatrix.AssertAlmostEqual(m, 1));
  }

  //[Theory]
  //[MemberData(nameof(Element1DTestsData.TestGetLocalStiffnessMatrixData), MemberType = typeof(Element1DTestsData))]
  //public void TestGetLocalStiffnessMatrix(double? E, double G, double? A, double Iz, double Iy, double J, double[] P0, double[] P1, double[,] matrixArray, bool exceptionThrown = false)
  //{
  //  var baseCurve = new Line(P0, P1);
  //  Material? mat = null;
  //  if (E is not null)
  //  {
  //    mat = new Material() { E = (double)E, G = G };
  //  }

  //  SectionProfile? section = null;
  //  if (A is not null)
  //  {
  //    section = new SectionProfile() { A = (double)A, Iz = Iz, Iy = Iy, J = J };
  //  }

  //  var element1D = new Element1D(baseCurve, section, mat);

  //  if (exceptionThrown)
  //  {
  //    _ = Assert.ThrowsAny<Exception>(() => element1D.LocalStiffnessMatrix);
  //    return;
  //  }
  //  var localStiffnessMatrix = element1D.LocalStiffnessMatrix;

  //  // element stiffness matrix should always be symmetric
  //  Assert.True(localStiffnessMatrix.IsSymmetric(), "Matrix is not symmetric");

  //  var expectedMatrix = DenseMatrix.OfArray(matrixArray);
  //  var equal = localStiffnessMatrix.AlmostEqual(expectedMatrix, .0001);

  //  Assert.True(equal, "Stiffness matrix is not equal to expected matrix");
  //}

  [SkippableTheory]
  [ClassData(typeof(AllElement1DFixtures))]
  [ClassData(typeof(TestGetLocalStiffnessMatrixData))]
  public void TestGetLocalStiffnessMatrix(Element1DFixture fixture)
  {
    var localStiffnessMatrix = fixture.Element.LocalStiffnessMatrix;

    _ = fixture.ExpectedLocalStiffnessMatrix.Match(
      m => localStiffnessMatrix.AssertAlmostEqual(m, 1),
      () => throw new SkipException("Insufficient expected data present on test fixture")
    );
  }

  [SkippableTheory]
  [ClassData(typeof(AllElement1DFixtures))]
  public void TestGetGlobalStiffnessMatrix(Element1DFixture fixture)
  {
    var element1D = fixture.Element;

    // this line isn't necessary but will help our codecov by initializing the local matrix
    var localStiffnessMatrix = element1D.LocalStiffnessMatrix;
    var globalStiffnessMatrix = element1D.GlobalStiffnessMatrix;

    // element stiffness matrix should always be symmetric
    globalStiffnessMatrix.AssertSymmetric();

    _ = fixture.ExpectedGlobalStiffnessMatrix.Match(
      m => globalStiffnessMatrix.AssertAlmostEqual(m, 1),
      () => throw new SkipException("Insufficient expected data present on test fixture")
    );
  }
}
