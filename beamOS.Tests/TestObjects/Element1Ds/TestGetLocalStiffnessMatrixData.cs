namespace beamOS.Tests.TestObjects.Element1Ds;
using System.Collections.Generic;
using System.Linq;
using beamOS.API.Schema.Objects;
using MathNet.Numerics.LinearAlgebra.Double;

internal class TestGetLocalStiffnessMatrixData : TheoryDataBase<Element1DFixture>
{
  public override List<Element1DFixture> AllTestObjects => TestFixtures().ToList();
  public static Line BaseLineUnit => new(new[] { 0.0, 0, 0 }, new[] { 1.0, 0, 0 });
  public static Material MaterialUnit => new("Unit", 1.0, 1.0);
  public static SectionProfile SectionProfileUnit => new("Unit", 1.0, 1.0, 1.0, 1.0);

  public static IEnumerable<Element1DFixture> TestFixtures()
  {
    yield return new Element1DFixture(new Element1D(
      BaseLineUnit,
      SectionProfileUnit,
      MaterialUnit
    ))
    {
      ExpectedLocalStiffnessMatrix = DenseMatrix.OfArray(new double[12, 12]
      {
        {  1,   0,   0,  0,  0,  0, -1,   0,   0,  0,  0,  0 },
        {  0,  12,   0,  0,  0,  6,  0, -12,   0,  0,  0,  6 },
        {  0,   0,  12,  0, -6,  0,  0,   0, -12,  0, -6,  0 },
        {  0,   0,   0,  1,  0,  0,  0,   0,   0, -1,  0,  0 },
        {  0,   0,  -6,  0,  4,  0,  0,   0,   6,  0,  2,  0 },
        {  0,   6,   0,  0,  0,  4,  0,  -6,   0,  0,  0,  2 },
        { -1,   0,   0,  0,  0,  0,  1,   0,   0,  0,  0,  0 },
        {  0, -12,   0,  0,  0, -6,  0,  12,   0,  0,  0, -6 },
        {  0,   0, -12,  0,  6,  0,  0,   0,  12,  0,  6,  0 },
        {  0,   0,   0, -1,  0,  0,  0,   0,   0,  1,  0,  0 },
        {  0,   0,  -6,  0,  2,  0,  0,   0,   6,  0,  4,  0 },
        {  0,   6,   0,  0,  0,  2,  0,  -6,   0,  0,  0,  4 },
      })
    };

    // Isolate the E variable
    yield return new Element1DFixture(new Element1D(
      BaseLineUnit,
      SectionProfileUnit,
      new Material(null, 5, 1)
    ))
    {
      ExpectedLocalStiffnessMatrix = DenseMatrix.OfArray(new double[12, 12]
      {
        {  5*1,   0,   0,  0,  0,  0, -5*1,   0,   0,  0,  0,  0 },
        {  0,  5*12,   0,  0,  0,  5*6,  0, -5*12,   0,  0,  0,  5*6 },
        {  0,   0,  5*12,  0, -5*6,  0,  0,   0, -5*12,  0, -5*6,  0 },
        {  0,   0,   0,  1,  0,  0,  0,   0,   0, -1,  0,  0 },
        {  0,   0,  -5*6,  0,  5*4,  0,  0,   0,   5*6,  0,  5*2,  0 },
        {  0,   5*6,   0,  0,  0,  5*4,  0,  -5*6,   0,  0,  0,  5*2 },
        { -5*1,   0,   0,  0,  0,  0,  5*1,   0,   0,  0,  0,  0 },
        {  0, -5*12,   0,  0,  0, -5*6,  0,  5*12,   0,  0,  0, -5*6 },
        {  0,   0, -5*12,  0,  5*6,  0,  0,   0,  5*12,  0,  5*6,  0 },
        {  0,   0,   0, -1,  0,  0,  0,   0,   0,  1,  0,  0 },
        {  0,   0,  -5*6,  0,  5*2,  0,  0,   0,   5*6,  0,  5*4,  0 },
        {  0,   5*6,   0,  0,  0,  5*2,  0,  -5*6,   0,  0,  0,  5*4 },
      })
    };

    //Isolate the G variable
    yield return new Element1DFixture(new Element1D(
      BaseLineUnit,
      SectionProfileUnit,
      new Material(null, 1, 5)
    ))
    {
      ExpectedLocalStiffnessMatrix = DenseMatrix.OfArray(new double[12, 12]
      {
        {  1,   0,   0,  0,  0,  0, -1,   0,   0,  0,  0,  0 },
        {  0,  12,   0,  0,  0,  6,  0, -12,   0,  0,  0,  6 },
        {  0,   0,  12,  0, -6,  0,  0,   0, -12,  0, -6,  0 },
        {  0,   0,   0,  5*1,  0,  0,  0,   0,   0, -5*1,  0,  0 },
        {  0,   0,  -6,  0,  4,  0,  0,   0,   6,  0,  2,  0 },
        {  0,   6,   0,  0,  0,  4,  0,  -6,   0,  0,  0,  2 },
        { -1,   0,   0,  0,  0,  0,  1,   0,   0,  0,  0,  0 },
        {  0, -12,   0,  0,  0, -6,  0,  12,   0,  0,  0, -6 },
        {  0,   0, -12,  0,  6,  0,  0,   0,  12,  0,  6,  0 },
        {  0,   0,   0, -5*1,  0,  0,  0,   0,   0,  5*1,  0,  0 },
        {  0,   0,  -6,  0,  2,  0,  0,   0,   6,  0,  4,  0 },
        {  0,   6,   0,  0,  0,  2,  0,  -6,   0,  0,  0,  4 },
      })
    };

    //Isolate the A variable
    yield return new Element1DFixture(new Element1D(
      BaseLineUnit,
      new SectionProfile(null, 1, 1, 5, 1),
      MaterialUnit
    ))
    {
      ExpectedLocalStiffnessMatrix = DenseMatrix.OfArray(new double[12, 12]
      {
        {  5*1,   0,   0,  0,  0,  0, -5*1,   0,   0,  0,  0,  0 },
        {  0,  12,   0,  0,  0,  6,  0, -12,   0,  0,  0,  6 },
        {  0,   0,  12,  0, -6,  0,  0,   0, -12,  0, -6,  0 },
        {  0,   0,   0,  1,  0,  0,  0,   0,   0, -1,  0,  0 },
        {  0,   0,  -6,  0,  4,  0,  0,   0,   6,  0,  2,  0 },
        {  0,   6,   0,  0,  0,  4,  0,  -6,   0,  0,  0,  2 },
        { -5*1,   0,   0,  0,  0,  0,  5*1,   0,   0,  0,  0,  0 },
        {  0, -12,   0,  0,  0, -6,  0,  12,   0,  0,  0, -6 },
        {  0,   0, -12,  0,  6,  0,  0,   0,  12,  0,  6,  0 },
        {  0,   0,   0, -1,  0,  0,  0,   0,   0,  1,  0,  0 },
        {  0,   0,  -6,  0,  2,  0,  0,   0,   6,  0,  4,  0 },
        {  0,   6,   0,  0,  0,  2,  0,  -6,   0,  0,  0,  4 },
      })
    };

    //Isolate the Iz variable
    yield return new Element1DFixture(new Element1D(
      BaseLineUnit,
      new SectionProfile(null, 5, 1, 1, 1),
      MaterialUnit
    ))
    {
      ExpectedLocalStiffnessMatrix = DenseMatrix.OfArray(new double[12, 12]
      {
        {  1,   0,   0,  0,  0,  0, -1,   0,   0,  0,  0,  0 },
        {  0,  5*12,   0,  0,  0,  5*6,  0, -5*12,   0,  0,  0,  5*6 },
        {  0,   0,  12,  0, -6,  0,  0,   0, -12,  0, -6,  0 },
        {  0,   0,   0,  1,  0,  0,  0,   0,   0, -1,  0,  0 },
        {  0,   0,  -6,  0,  4,  0,  0,   0,   6,  0,  2,  0 },
        {  0,   5*6,   0,  0,  0,  5*4,  0,  -5*6,   0,  0,  0,  5*2 },
        { -1,   0,   0,  0,  0,  0,  1,   0,   0,  0,  0,  0 },
        {  0, -5*12,   0,  0,  0, -5*6,  0,  5*12,   0,  0,  0, -5*6 },
        {  0,   0, -12,  0,  6,  0,  0,   0,  12,  0,  6,  0 },
        {  0,   0,   0, -1,  0,  0,  0,   0,   0,  1,  0,  0 },
        {  0,   0,  -6,  0,  2,  0,  0,   0,   6,  0,  4,  0 },
        {  0,   5*6,   0,  0,  0,  5*2,  0,  -5*6,   0,  0,  0,  5*4 },
      })
    };

    //Isolate the Iy variable
    yield return new Element1DFixture(new Element1D(
      BaseLineUnit,
      new SectionProfile(null, 1, 5, 1, 1),
      MaterialUnit
    ))
    {
      ExpectedLocalStiffnessMatrix = DenseMatrix.OfArray(new double[12, 12]
      {
        {  1,   0,   0,  0,  0,  0, -1,   0,   0,  0,  0,  0 },
        {  0,  12,   0,  0,  0,  6,  0, -12,   0,  0,  0,  6 },
        {  0,   0,  5*12,  0, -5*6,  0,  0,   0, -5*12,  0, -5*6,  0 },
        {  0,   0,   0,  1,  0,  0,  0,   0,   0, -1,  0,  0 },
        {  0,   0,  -5*6,  0,  5*4,  0,  0,   0,   5*6,  0,  5*2,  0 },
        {  0,   6,   0,  0,  0,  4,  0,  -6,   0,  0,  0,  2 },
        { -1,   0,   0,  0,  0,  0,  1,   0,   0,  0,  0,  0 },
        {  0, -12,   0,  0,  0, -6,  0,  12,   0,  0,  0, -6 },
        {  0,   0, -5*12,  0,  5*6,  0,  0,   0,  5*12,  0,  5*6,  0 },
        {  0,   0,   0, -1,  0,  0,  0,   0,   0,  1,  0,  0 },
        {  0,   0,  -5*6,  0,  5*2,  0,  0,   0,   5*6,  0,  5*4,  0 },
        {  0,   6,   0,  0,  0,  2,  0,  -6,   0,  0,  0,  4 },
      })
    };

    //Isolate the J variable
    yield return new Element1DFixture(new Element1D(
      BaseLineUnit,
      new SectionProfile(null, 1, 1, 1, 5),
      MaterialUnit
    ))
    {
      ExpectedLocalStiffnessMatrix = DenseMatrix.OfArray(new double[12, 12]
      {
        {  1,   0,   0,  0,  0,  0, -1,   0,   0,  0,  0,  0 },
        {  0,  12,   0,  0,  0,  6,  0, -12,   0,  0,  0,  6 },
        {  0,   0,  12,  0, -6,  0,  0,   0, -12,  0, -6,  0 },
        {  0,   0,   0,  5*1,  0,  0,  0,   0,   0, -5*1,  0,  0 },
        {  0,   0,  -6,  0,  4,  0,  0,   0,   6,  0,  2,  0 },
        {  0,   6,   0,  0,  0,  4,  0,  -6,   0,  0,  0,  2 },
        { -1,   0,   0,  0,  0,  0,  1,   0,   0,  0,  0,  0 },
        {  0, -12,   0,  0,  0, -6,  0,  12,   0,  0,  0, -6 },
        {  0,   0, -12,  0,  6,  0,  0,   0,  12,  0,  6,  0 },
        {  0,   0,   0, -5*1,  0,  0,  0,   0,   0,  5*1,  0,  0 },
        {  0,   0,  -6,  0,  2,  0,  0,   0,   6,  0,  4,  0 },
        {  0,   6,   0,  0,  0,  2,  0,  -6,   0,  0,  0,  4 },
      })
    };

    //Isolate the L variable
    yield return new Element1DFixture(new Element1D(
      new Line(new double[] { 0, 0, 0 }, new double[] { 5, 0, 0 }),
      SectionProfileUnit,
      MaterialUnit
    ))
    {
      ExpectedLocalStiffnessMatrix = DenseMatrix.OfArray(new double[12, 12]
      {
        {  1/5.0,   0,   0,  0,  0,  0, -1/5.0,   0,   0,  0,  0,  0 },
        {  0,  12/125.0,   0,  0,  0,  6/25.0,  0, -12/125.0,   0,  0,  0,  6/25.0 },
        {  0,   0,  12/125.0,  0, -6/25.0,  0,  0,   0, -12/125.0,  0, -6/25.0,  0 },
        {  0,   0,   0,  1/5.0,  0,  0,  0,   0,   0, -1/5.0,  0,  0 },
        {  0,   0,  -6/25.0,  0,  4/5.0,  0,  0,   0,   6/25.0,  0,  2/5.0,  0 },
        {  0,   6/25.0,   0,  0,  0,  4/5.0,  0,  -6/25.0,   0,  0,  0,  2/5.0 },
        { -1/5.0,   0,   0,  0,  0,  0,  1/5.0,   0,   0,  0,  0,  0 },
        {  0, -12/125.0,   0,  0,  0, -6/25.0,  0,  12/125.0,   0,  0,  0, -6/25.0 },
        {  0,   0, -12/125.0,  0,  6/25.0,  0,  0,   0,  12/125.0,  0,  6/25.0,  0 },
        {  0,   0,   0, -1/5.0,  0,  0,  0,   0,   0,  1/5.0,  0,  0 },
        {  0,   0,  -6/25.0,  0,  2/5.0,  0,  0,   0,   6/25.0,  0,  4/5.0,  0 },
        {  0,   6/25.0,   0,  0,  0,  2/5.0,  0,  -6/25.0,   0,  0,  0,  4/5.0 },
      })
    };
  }
}
