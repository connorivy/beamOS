using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Extensions;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Factories;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalElement1Ds;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Throw;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Element1DAggregate;

public partial class Element1DTests
{
    [SkippableTheory]
    [ClassData(typeof(AllElement1DFixtures))]
    public void GetStiffnessMatrix_ForAllElement1DFixtures_ShouldEqualExpectedValue(
        AnalyticalElement1DFixture fixture
    )
    {
        _ = fixture.ExpectedLocalStiffnessMatrix.ThrowIfNull(() => throw new SkipException());

        Matrix<double> localStiffnessMatrix = fixture
            .Element
            .GetLocalStiffnessMatrix(
                fixture.UnitSettings.ForceUnit,
                fixture.UnitSettings.ForcePerLengthUnit,
                fixture.UnitSettings.TorqueUnit
            );

        localStiffnessMatrix.AssertAlmostEqual(fixture.ExpectedLocalStiffnessMatrix, 1);
    }

    [Fact]
    public void GetStiffnessMatrix_WithAllUnitValue_ShouldEqualCoefficients()
    {
        AnalyticalElement1D element = Element1DFactory.Create();

        Matrix<double> calculatedLocalStiffnessMatrix = element.GetLocalStiffnessMatrix(
            UnitSettings.SI.ForceUnit,
            UnitSettings.SI.ForcePerLengthUnit,
            UnitSettings.SI.TorqueUnit
        );
        Matrix<double> expectedLocalStiffnessMatrix = DenseMatrix.OfArray(
            new double[12, 12]
            {
                { 1, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0 },
                { 0, 12, 0, 0, 0, 6, 0, -12, 0, 0, 0, 6 },
                { 0, 0, 12, 0, -6, 0, 0, 0, -12, 0, -6, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 0, 0, -1, 0, 0 },
                { 0, 0, -6, 0, 4, 0, 0, 0, 6, 0, 2, 0 },
                { 0, 6, 0, 0, 0, 4, 0, -6, 0, 0, 0, 2 },
                { -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, -12, 0, 0, 0, -6, 0, 12, 0, 0, 0, -6 },
                { 0, 0, -12, 0, 6, 0, 0, 0, 12, 0, 6, 0 },
                { 0, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0 },
                { 0, 0, -6, 0, 2, 0, 0, 0, 6, 0, 4, 0 },
                { 0, 6, 0, 0, 0, 2, 0, -6, 0, 0, 0, 4 },
            }
        );

        calculatedLocalStiffnessMatrix.AssertAlmostEqual(expectedLocalStiffnessMatrix);
    }

    [Fact]
    public void GetStiffnessMatrix_WithIsolatedEVariable_ShouldEqualExpectedValue()
    {
        Material material = MaterialFactory.Create(
            modulusOfElasticity: new Pressure(5, UnitSystem.SI)
        );
        AnalyticalElement1D element = Element1DFactory.Create(material: material);

        Matrix<double> calculatedLocalStiffnessMatrix = element.GetLocalStiffnessMatrix(
            UnitSettings.SI.ForceUnit,
            UnitSettings.SI.ForcePerLengthUnit,
            UnitSettings.SI.TorqueUnit
        );
        Matrix<double> expectedLocalStiffnessMatrix = DenseMatrix.OfArray(
            new double[12, 12]
            {
                { 5 * 1, 0, 0, 0, 0, 0, -5 * 1, 0, 0, 0, 0, 0 },
                { 0, 5 * 12, 0, 0, 0, 5 * 6, 0, -5 * 12, 0, 0, 0, 5 * 6 },
                { 0, 0, 5 * 12, 0, -5 * 6, 0, 0, 0, -5 * 12, 0, -5 * 6, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 0, 0, -1, 0, 0 },
                { 0, 0, -5 * 6, 0, 5 * 4, 0, 0, 0, 5 * 6, 0, 5 * 2, 0 },
                { 0, 5 * 6, 0, 0, 0, 5 * 4, 0, -5 * 6, 0, 0, 0, 5 * 2 },
                { -5 * 1, 0, 0, 0, 0, 0, 5 * 1, 0, 0, 0, 0, 0 },
                { 0, -5 * 12, 0, 0, 0, -5 * 6, 0, 5 * 12, 0, 0, 0, -5 * 6 },
                { 0, 0, -5 * 12, 0, 5 * 6, 0, 0, 0, 5 * 12, 0, 5 * 6, 0 },
                { 0, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0 },
                { 0, 0, -5 * 6, 0, 5 * 2, 0, 0, 0, 5 * 6, 0, 5 * 4, 0 },
                { 0, 5 * 6, 0, 0, 0, 5 * 2, 0, -5 * 6, 0, 0, 0, 5 * 4 },
            }
        );

        calculatedLocalStiffnessMatrix.AssertAlmostEqual(expectedLocalStiffnessMatrix);
    }

    [Fact]
    public void GetStiffnessMatrix_WithIsolatedGVariable_ShouldEqualExpectedValue()
    {
        Material material = MaterialFactory.Create(
            modulusOfRigidity: new Pressure(5, UnitSystem.SI)
        );
        AnalyticalElement1D element = Element1DFactory.Create(material: material);

        Matrix<double> calculatedLocalStiffnessMatrix = element.GetLocalStiffnessMatrix(
            UnitSettings.SI.ForceUnit,
            UnitSettings.SI.ForcePerLengthUnit,
            UnitSettings.SI.TorqueUnit
        );
        Matrix<double> expectedLocalStiffnessMatrix = DenseMatrix.OfArray(
            new double[12, 12]
            {
                { 1, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0 },
                { 0, 12, 0, 0, 0, 6, 0, -12, 0, 0, 0, 6 },
                { 0, 0, 12, 0, -6, 0, 0, 0, -12, 0, -6, 0 },
                { 0, 0, 0, 5 * 1, 0, 0, 0, 0, 0, -5 * 1, 0, 0 },
                { 0, 0, -6, 0, 4, 0, 0, 0, 6, 0, 2, 0 },
                { 0, 6, 0, 0, 0, 4, 0, -6, 0, 0, 0, 2 },
                { -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, -12, 0, 0, 0, -6, 0, 12, 0, 0, 0, -6 },
                { 0, 0, -12, 0, 6, 0, 0, 0, 12, 0, 6, 0 },
                { 0, 0, 0, -5 * 1, 0, 0, 0, 0, 0, 5 * 1, 0, 0 },
                { 0, 0, -6, 0, 2, 0, 0, 0, 6, 0, 4, 0 },
                { 0, 6, 0, 0, 0, 2, 0, -6, 0, 0, 0, 4 },
            }
        );

        calculatedLocalStiffnessMatrix.AssertAlmostEqual(expectedLocalStiffnessMatrix);
    }

    [Fact]
    public void GetStiffnessMatrix_WithIsolatedAVariable_ShouldEqualExpectedValue()
    {
        SectionProfile section = SectionProfileFactory.CreateSI(area: new Area(5, UnitSystem.SI));
        AnalyticalElement1D element = Element1DFactory.Create(sectionProfile: section);

        Matrix<double> calculatedLocalStiffnessMatrix = element.GetLocalStiffnessMatrix(
            UnitSettings.SI.ForceUnit,
            UnitSettings.SI.ForcePerLengthUnit,
            UnitSettings.SI.TorqueUnit
        );
        Matrix<double> expectedLocalStiffnessMatrix = DenseMatrix.OfArray(
            new double[12, 12]
            {
                { 5 * 1, 0, 0, 0, 0, 0, -5 * 1, 0, 0, 0, 0, 0 },
                { 0, 12, 0, 0, 0, 6, 0, -12, 0, 0, 0, 6 },
                { 0, 0, 12, 0, -6, 0, 0, 0, -12, 0, -6, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 0, 0, -1, 0, 0 },
                { 0, 0, -6, 0, 4, 0, 0, 0, 6, 0, 2, 0 },
                { 0, 6, 0, 0, 0, 4, 0, -6, 0, 0, 0, 2 },
                { -5 * 1, 0, 0, 0, 0, 0, 5 * 1, 0, 0, 0, 0, 0 },
                { 0, -12, 0, 0, 0, -6, 0, 12, 0, 0, 0, -6 },
                { 0, 0, -12, 0, 6, 0, 0, 0, 12, 0, 6, 0 },
                { 0, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0 },
                { 0, 0, -6, 0, 2, 0, 0, 0, 6, 0, 4, 0 },
                { 0, 6, 0, 0, 0, 2, 0, -6, 0, 0, 0, 4 },
            }
        );

        calculatedLocalStiffnessMatrix.AssertAlmostEqual(expectedLocalStiffnessMatrix);
    }

    [Fact]
    public void GetStiffnessMatrix_WithIsolatedIStrongVariable_ShouldEqualExpectedValue()
    {
        SectionProfile section = SectionProfileFactory.CreateSI(
            strongAxisMomentOfInertia: new AreaMomentOfInertia(5, UnitSystem.SI)
        );
        AnalyticalElement1D element = Element1DFactory.Create(sectionProfile: section);

        Matrix<double> calculatedLocalStiffnessMatrix = element.GetLocalStiffnessMatrix(
            UnitSettings.SI.ForceUnit,
            UnitSettings.SI.ForcePerLengthUnit,
            UnitSettings.SI.TorqueUnit
        );
        Matrix<double> expectedLocalStiffnessMatrix = DenseMatrix.OfArray(
            new double[12, 12]
            {
                { 1, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0 },
                { 0, 5 * 12, 0, 0, 0, 5 * 6, 0, -5 * 12, 0, 0, 0, 5 * 6 },
                { 0, 0, 12, 0, -6, 0, 0, 0, -12, 0, -6, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 0, 0, -1, 0, 0 },
                { 0, 0, -6, 0, 4, 0, 0, 0, 6, 0, 2, 0 },
                { 0, 5 * 6, 0, 0, 0, 5 * 4, 0, -5 * 6, 0, 0, 0, 5 * 2 },
                { -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, -5 * 12, 0, 0, 0, -5 * 6, 0, 5 * 12, 0, 0, 0, -5 * 6 },
                { 0, 0, -12, 0, 6, 0, 0, 0, 12, 0, 6, 0 },
                { 0, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0 },
                { 0, 0, -6, 0, 2, 0, 0, 0, 6, 0, 4, 0 },
                { 0, 5 * 6, 0, 0, 0, 5 * 2, 0, -5 * 6, 0, 0, 0, 5 * 4 },
            }
        );

        calculatedLocalStiffnessMatrix.AssertAlmostEqual(expectedLocalStiffnessMatrix);
    }

    [Fact]
    public void GetStiffnessMatrix_WithIsolatedIWeakVariable_ShouldEqualExpectedValue()
    {
        SectionProfile section = SectionProfileFactory.CreateSI(
            weakAxisMomentOfInertia: new AreaMomentOfInertia(5, UnitSystem.SI)
        );
        AnalyticalElement1D element = Element1DFactory.Create(sectionProfile: section);

        Matrix<double> calculatedLocalStiffnessMatrix = element.GetLocalStiffnessMatrix(
            UnitSettings.SI.ForceUnit,
            UnitSettings.SI.ForcePerLengthUnit,
            UnitSettings.SI.TorqueUnit
        );
        Matrix<double> expectedLocalStiffnessMatrix = DenseMatrix.OfArray(
            new double[12, 12]
            {
                { 1, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0 },
                { 0, 12, 0, 0, 0, 6, 0, -12, 0, 0, 0, 6 },
                { 0, 0, 5 * 12, 0, -5 * 6, 0, 0, 0, -5 * 12, 0, -5 * 6, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 0, 0, -1, 0, 0 },
                { 0, 0, -5 * 6, 0, 5 * 4, 0, 0, 0, 5 * 6, 0, 5 * 2, 0 },
                { 0, 6, 0, 0, 0, 4, 0, -6, 0, 0, 0, 2 },
                { -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, -12, 0, 0, 0, -6, 0, 12, 0, 0, 0, -6 },
                { 0, 0, -5 * 12, 0, 5 * 6, 0, 0, 0, 5 * 12, 0, 5 * 6, 0 },
                { 0, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0 },
                { 0, 0, -5 * 6, 0, 5 * 2, 0, 0, 0, 5 * 6, 0, 5 * 4, 0 },
                { 0, 6, 0, 0, 0, 2, 0, -6, 0, 0, 0, 4 },
            }
        );

        calculatedLocalStiffnessMatrix.AssertAlmostEqual(expectedLocalStiffnessMatrix);
    }

    [Fact]
    public void GetStiffnessMatrix_WithIsolatedJVariable_ShouldEqualExpectedValue()
    {
        SectionProfile section = SectionProfileFactory.CreateSI(
            polarMomentOfInertia: new AreaMomentOfInertia(5, UnitSystem.SI)
        );
        AnalyticalElement1D element = Element1DFactory.Create(sectionProfile: section);

        Matrix<double> calculatedLocalStiffnessMatrix = element.GetLocalStiffnessMatrix(
            UnitSettings.SI.ForceUnit,
            UnitSettings.SI.ForcePerLengthUnit,
            UnitSettings.SI.TorqueUnit
        );
        Matrix<double> expectedLocalStiffnessMatrix = DenseMatrix.OfArray(
            new double[12, 12]
            {
                { 1, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0 },
                { 0, 12, 0, 0, 0, 6, 0, -12, 0, 0, 0, 6 },
                { 0, 0, 12, 0, -6, 0, 0, 0, -12, 0, -6, 0 },
                { 0, 0, 0, 5 * 1, 0, 0, 0, 0, 0, -5 * 1, 0, 0 },
                { 0, 0, -6, 0, 4, 0, 0, 0, 6, 0, 2, 0 },
                { 0, 6, 0, 0, 0, 4, 0, -6, 0, 0, 0, 2 },
                { -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, -12, 0, 0, 0, -6, 0, 12, 0, 0, 0, -6 },
                { 0, 0, -12, 0, 6, 0, 0, 0, 12, 0, 6, 0 },
                { 0, 0, 0, -5 * 1, 0, 0, 0, 0, 0, 5 * 1, 0, 0 },
                { 0, 0, -6, 0, 2, 0, 0, 0, 6, 0, 4, 0 },
                { 0, 6, 0, 0, 0, 2, 0, -6, 0, 0, 0, 4 },
            }
        );

        calculatedLocalStiffnessMatrix.AssertAlmostEqual(expectedLocalStiffnessMatrix);
    }

    [Fact]
    public void GetStiffnessMatrix_WithIsolatedLVariable_ShouldEqualExpectedValue()
    {
        AnalyticalElement1D element = Element1DFactory.Create(
            startNode: new(0, 0, 0, UnitsNet.Units.LengthUnit.Meter, Restraint.Free),
            endNode: new(5, 0, 0, UnitsNet.Units.LengthUnit.Meter, Restraint.Free)
        );

        Matrix<double> calculatedLocalStiffnessMatrix = element.GetLocalStiffnessMatrix(
            UnitSettings.SI.ForceUnit,
            UnitSettings.SI.ForcePerLengthUnit,
            UnitSettings.SI.TorqueUnit
        );
        Matrix<double> expectedLocalStiffnessMatrix = DenseMatrix.OfArray(
            new double[12, 12]
            {
                { 1 / 5.0, 0, 0, 0, 0, 0, -1 / 5.0, 0, 0, 0, 0, 0 },
                { 0, 12 / 125.0, 0, 0, 0, 6 / 25.0, 0, -12 / 125.0, 0, 0, 0, 6 / 25.0 },
                { 0, 0, 12 / 125.0, 0, -6 / 25.0, 0, 0, 0, -12 / 125.0, 0, -6 / 25.0, 0 },
                { 0, 0, 0, 1 / 5.0, 0, 0, 0, 0, 0, -1 / 5.0, 0, 0 },
                { 0, 0, -6 / 25.0, 0, 4 / 5.0, 0, 0, 0, 6 / 25.0, 0, 2 / 5.0, 0 },
                { 0, 6 / 25.0, 0, 0, 0, 4 / 5.0, 0, -6 / 25.0, 0, 0, 0, 2 / 5.0 },
                { -1 / 5.0, 0, 0, 0, 0, 0, 1 / 5.0, 0, 0, 0, 0, 0 },
                { 0, -12 / 125.0, 0, 0, 0, -6 / 25.0, 0, 12 / 125.0, 0, 0, 0, -6 / 25.0 },
                { 0, 0, -12 / 125.0, 0, 6 / 25.0, 0, 0, 0, 12 / 125.0, 0, 6 / 25.0, 0 },
                { 0, 0, 0, -1 / 5.0, 0, 0, 0, 0, 0, 1 / 5.0, 0, 0 },
                { 0, 0, -6 / 25.0, 0, 2 / 5.0, 0, 0, 0, 6 / 25.0, 0, 4 / 5.0, 0 },
                { 0, 6 / 25.0, 0, 0, 0, 2 / 5.0, 0, -6 / 25.0, 0, 0, 0, 4 / 5.0 },
            }
        );

        calculatedLocalStiffnessMatrix.AssertAlmostEqual(expectedLocalStiffnessMatrix);
    }
}
