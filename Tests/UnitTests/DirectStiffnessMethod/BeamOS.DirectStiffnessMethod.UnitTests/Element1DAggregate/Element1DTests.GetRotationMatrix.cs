using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Extensions;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Factories;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalElement1Ds;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Throw;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Element1DAggregate;
public partial class Element1DTests
{
    [SkippableTheory]
    [ClassData(typeof(AllElement1DFixtures))]
    public void GetRotationMatrix_ForAllElement1DFixtures_ShouldEqualExpectedValue(
    AnalyticalElement1DFixture fixture
  )
    {
        _ = fixture.ExpectedRotationMatrix.ThrowIfNull(() => throw new SkipException());

        Matrix<double> rotationMatrix = fixture.Element.GetRotationMatrix();

        rotationMatrix.AssertAlmostEqual(fixture.ExpectedRotationMatrix);
    }

    [Fact]
    public void GetRotationMatrix_AlignedWithGlobalCoords_ShouldEqualExpectedValue()
    {
        // if the beam is oriented in the same direction as the global coordinate system,
        // then the unit vectors of the global domain should be returned
        var startNode = AnalyticalNode.Create(10, 7, -3, LengthUnit.Foot);
        var endNode = AnalyticalNode.Create(20, 7, -3, LengthUnit.Foot);
        AnalyticalElement1D element = Element1DFactory.Create(startNode: startNode, endNode: endNode);

        Matrix<double> expectedRotationMatrix = DenseMatrix.OfArray(new[,]
        {
            { 1.0, 0, 0},
            { 0, 1.0, 0},
            { 0, 0, 1.0},
        });

        element.GetRotationMatrix().AssertAlmostEqual(expectedRotationMatrix);
    }

    [Theory]
    [InlineData(10, 10, 5, 20, 18, 5)]
    //[InlineData(10, 10, 5, -12, 8, 5)] TODO : fix this guy using sin / cos instead of sqrtCx2Cz2
    public void GetRotationMatrix_ParallelToGlobalXYPlane_ShouldEqualExpectedValue(
        double x0,
        double y0,
        double z0,
        double x1,
        double y1,
        double z1)
    {
        // if an elements local xy plane is equal to or parallel with the global xy plane,
        // return the following matrix (ref Advanced Structural Analysis with MATLAB eqn 4.17)
        AnalyticalNode startNode = AnalyticalNode.Create(x0, y0, z0, LengthUnit.Foot);
        AnalyticalNode endNode = AnalyticalNode.Create(x1, y1, z1, LengthUnit.Foot);
        AnalyticalElement1D element = Element1DFactory.Create(startNode: startNode, endNode: endNode);

        var L = element.Length;
        var cx = (endNode.LocationPoint.XCoordinate - startNode.LocationPoint.XCoordinate) / L;
        var cy = (endNode.LocationPoint.YCoordinate - startNode.LocationPoint.YCoordinate) / L;
        var cz = (endNode.LocationPoint.ZCoordinate - startNode.LocationPoint.ZCoordinate) / L;
        var sqrtCx2Cz2 = Math.Sqrt(Math.Pow(cx, 2) + Math.Pow(cz, 2));
        Matrix<double> expectedRotationMatrix = DenseMatrix.OfArray(new[,]
        {
            {  sqrtCx2Cz2,         cy, 0},
            {         -cy, sqrtCx2Cz2, 0},
            {           0,          0, 1},
        });

        element.GetRotationMatrix().AssertAlmostEqual(expectedRotationMatrix);
    }

    [Theory]
    [InlineData(10, 5, 10, 20, 5, 18)]
    [InlineData(10, 5, 10, -12, 5, 8)]
    public void GetRotationMatrix_ParallelToGlobalXZPlane_ShouldEqualExpectedValue(
        double x0,
        double y0,
        double z0,
        double x1,
        double y1,
        double z1)
    {
        // if an elements local xz is equal to or parallel with the global xz plane,
        // return the following matrix (ref Advanced Structural Analysis with MATLAB eqn 4.16)
        AnalyticalNode startNode = AnalyticalNode.Create(x0, y0, z0, LengthUnit.Foot);
        AnalyticalNode endNode = AnalyticalNode.Create(x1, y1, z1, LengthUnit.Foot);
        AnalyticalElement1D element = Element1DFactory.Create(startNode: startNode, endNode: endNode);

        var L = element.Length;
        var cx = (endNode.LocationPoint.XCoordinate - startNode.LocationPoint.XCoordinate) / L;
        var cz = (endNode.LocationPoint.ZCoordinate - startNode.LocationPoint.ZCoordinate) / L;
        var sqrtCx2Cz2 = Math.Sqrt(Math.Pow(cx, 2) + Math.Pow(cz, 2));
        Matrix<double> expectedRotationMatrix = DenseMatrix.OfArray(new[,]
        {
            {   cx / sqrtCx2Cz2, 0, cz / sqrtCx2Cz2},
            {                 0, 1,               0},
            { - cz / sqrtCx2Cz2, 0, cx / sqrtCx2Cz2},
        });

        element.GetRotationMatrix().AssertAlmostEqual(expectedRotationMatrix);
    }

    [Theory]
    [InlineData(10, 18, -15, 20, 18, -15, 90)]
    [InlineData(10, 18, -15, 20, 18, -15, -36)]
    public void GetRotationMatrix_AlignedWithGlobalCoordsRotated_ShouldEqualExpectedValue(
        double x0,
        double y0,
        double z0,
        double x1,
        double y1,
        double z1,
        double rotationDegrees)
    {
        // if an element is aligned with the global coord system, but has a non 0 rotation,
        // return the following matrix (ref Advanced Structural Analysis with MATLAB eqn 4.18)
        Angle rotation = new(rotationDegrees, AngleUnit.Degree);
        AnalyticalNode startNode = AnalyticalNode.Create(x0, y0, z0, LengthUnit.Foot);
        AnalyticalNode endNode = AnalyticalNode.Create(x1, y1, z1, LengthUnit.Foot);
        AnalyticalElement1D element = Element1DFactory.Create(
          startNode: startNode,
          endNode: endNode,
          rotation: rotation
        );

        Matrix<double> expectedRotationMatrix = DenseMatrix.OfArray(new[,]
        {
            { 1, 0, 0},
            { 0, Math.Cos(rotation.Radians), Math.Sin(rotation.Radians)},
            { 0, -Math.Sin(rotation.Radians), Math.Cos(rotation.Radians)},
        });

        element.GetRotationMatrix().AssertAlmostEqual(expectedRotationMatrix);
    }

    [Fact]
    public void GetRotationMatrix_AlignedWithGlobalPlanesRotateMinus36Degree_ShouldEqualExpectedValue()
    {
        Angle rotation = new(-36, AngleUnit.Degree);
        AnalyticalNode startNode = AnalyticalNode.Create(10, 18, -15, LengthUnit.Foot);
        AnalyticalNode endNode = AnalyticalNode.Create(20, 18, -15, LengthUnit.Foot);
        AnalyticalElement1D element = Element1DFactory.Create(
          startNode: startNode,
          endNode: endNode,
          rotation: rotation
        );

        Matrix<double> expectedRotationMatrix = DenseMatrix.OfArray(new[,]
        {
            { 1, 0, 0},
            { 0, Math.Cos(rotation.Radians), Math.Sin(rotation.Radians)},
            { 0, -Math.Sin(rotation.Radians), Math.Cos(rotation.Radians)},
        });

        element.GetRotationMatrix().AssertAlmostEqual(expectedRotationMatrix);
    }

    [Fact]
    public void GetRotationMatrix_VerticalSimple_ShouldEqualExpectedValue()
    {
        // by default (aka no profile rotation), a vertical member will end up with it's local
        // y axis aligned in the global -x direction
        AnalyticalNode startNode = AnalyticalNode.Create(10, 10, 5, LengthUnit.Foot);
        AnalyticalNode endNode = AnalyticalNode.Create(10, 18, 5, LengthUnit.Foot);
        AnalyticalElement1D element = Element1DFactory.Create(startNode: startNode, endNode: endNode);

        Matrix<double> expectedRotationMatrix = DenseMatrix.OfArray(new[,]
        {
            { 0.0, 1, 0 },
            {  -1, 0, 0 },
            {   0, 0, 1 },
        });

        element.GetRotationMatrix().AssertAlmostEqual(expectedRotationMatrix);
    }

    [Fact]
    public void GetRotationMatrix_VerticalMemberRotated90_ShouldEqualExpectedValue()
    {
        // a positive (counter clockwise) 90 degree rotation will align the local y axis in the global +z direction
        Angle rotation = new(90, AngleUnit.Degree);
        AnalyticalNode startNode = AnalyticalNode.Create(-9, -7, 5, LengthUnit.Foot);
        AnalyticalNode endNode = AnalyticalNode.Create(-9, 0, 5, LengthUnit.Foot);
        AnalyticalElement1D element = Element1DFactory.Create(
          startNode: startNode,
          endNode: endNode,
          rotation: rotation
        );

        Matrix<double> expectedRotationMatrix = DenseMatrix.OfArray(new[,]
        {
            { 0, 1.0, 0 },
            { 0, 0, 1.0 },
            { 1.0, 0, 0 }
        });

        element.GetRotationMatrix().AssertAlmostEqual(expectedRotationMatrix);
    }

    [Fact]
    public void GetRotationMatrix_VerticalMemberRotatedMinus30_ShouldEqualExpectedValue()
    {
        // a negative (clockwise) 30 degree rotation will rotate the local y axis 30 degrees counter clockwise from the global -x direction
        // the local y axis will be -cos(-30d) in the global x direction and sin(-30d) in the global z dir
        // the local z axis will be sin(-30d) in the global x direction and cos(-30d) in the global z dir
        Angle rotation = new(-30, AngleUnit.Degree);
        AnalyticalNode startNode = AnalyticalNode.Create(10, -7, -15, LengthUnit.Foot);
        AnalyticalNode endNode = AnalyticalNode.Create(10, 18, -15, LengthUnit.Foot);
        AnalyticalElement1D element = Element1DFactory.Create(
          startNode: startNode,
          endNode: endNode,
          rotation: rotation
        );

        Matrix<double> expectedRotationMatrix = DenseMatrix.OfArray(new[,]
        {
            { 0, 1.0, 0 },
	        // -1.0 * cos(-30d), 0, sin(-30d)
	        { -0.86602540378, 0, -.5 },
	        // 1.0 * sin(-30d), 0, cos(-30d)
	        { -.5, 0, 0.86602540378 },
        });

        element.GetRotationMatrix().AssertAlmostEqual(expectedRotationMatrix);
    }

    [Fact]
    public void GetRotationMatrix_VerticalUpsideDown_ShouldEqualExpectedValue()
    {
        // a vertical member that has point 0 above point 1 will be aligned in the global -x dir
        // the local y axis will be in the global +x dir and the local z will be in the global +z
        AnalyticalNode startNode = AnalyticalNode.Create(10, 10, 5, LengthUnit.Foot);
        AnalyticalNode endNode = AnalyticalNode.Create(10, 18, 5, LengthUnit.Foot);
        AnalyticalElement1D element = Element1DFactory.Create(startNode: startNode, endNode: endNode);

        Matrix<double> expectedRotationMatrix = DenseMatrix.OfArray(new[,]
        {
            { 0.0, 1, 0 },
            {  -1, 0, 0 },
            {   0, 0, 1 },
        });

        element.GetRotationMatrix().AssertAlmostEqual(expectedRotationMatrix);
    }

    [Fact]
    public void GetRotationMatrix_VerticalUpsideDownRotated_ShouldEqualExpectedValue()
    {
        // for an element aligned with the global -y axis,
        // a positive (counter clockwise) 90 degree rotation will align the local y axis in the global +z direction
        Angle rotation = new(90, AngleUnit.Degree);
        AnalyticalNode startNode = AnalyticalNode.Create(10, 36, -15, LengthUnit.Foot);
        AnalyticalNode endNode = AnalyticalNode.Create(10, 18, -15, LengthUnit.Foot);
        AnalyticalElement1D element = Element1DFactory.Create(
          startNode: startNode,
          endNode: endNode,
          rotation: rotation
        );

        Matrix<double> expectedRotationMatrix = DenseMatrix.OfArray(new[,]
        {
            { 0, -1.0, 0 },
            { 0, 0, 1.0 },
            { -1.0, 0, 0 },
        });

        element.GetRotationMatrix().AssertAlmostEqual(expectedRotationMatrix);
    }

    [Fact]
    public void GetRotationMatrix_MisalignedFromGlobal_ShouldEqualExpectedValue()
    {
        // simplest case
        AnalyticalNode startNode = AnalyticalNode.Create(0, 0, 0, LengthUnit.Foot);
        AnalyticalNode endNode = AnalyticalNode.Create(1, 1, 1, LengthUnit.Foot);
        AnalyticalElement1D element = Element1DFactory.Create(startNode: startNode, endNode: endNode);

        Matrix<double> expectedRotationMatrix = DenseMatrix.OfArray(new[,]
        {
            { 0.57735026919, 0.57735026919, 0.57735026919},
            { -0.408248,  0.816497,  -0.408248},
            { -0.70710678118, 0, 0.70710678118},
        });

        element.GetRotationMatrix().AssertAlmostEqual(expectedRotationMatrix);
    }

    [Fact]
    public void GetRotationMatrix_MisalignedFromGlobalComplex_ShouldEqualExpectedValue()
    {
        // This answer is taken from Matrix Analysis of Structures example 8.3
        Angle rotation = new(0.857302717, AngleUnit.Radian);
        AnalyticalNode startNode = AnalyticalNode.Create(4, 7, 6, LengthUnit.Foot);
        AnalyticalNode endNode = AnalyticalNode.Create(20, 15, 17, LengthUnit.Foot);
        AnalyticalElement1D element = Element1DFactory.Create(
          startNode: startNode,
          endNode: endNode,
          rotation: rotation
        );

        Matrix<double> expectedRotationMatrix = DenseMatrix.OfArray(new[,]
        {
            { 0.7619, 0.38095, 0.52381},
            { -0.6338, 0.60512, 0.48181},
            { -0.13343, -0.69909, 0.70249},
        });

        element.GetRotationMatrix().AssertAlmostEqual(expectedRotationMatrix);
    }
}
