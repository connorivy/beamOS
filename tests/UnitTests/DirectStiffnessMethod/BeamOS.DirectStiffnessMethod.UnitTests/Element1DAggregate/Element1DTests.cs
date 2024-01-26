using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Extensions;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalElement1Ds;
using MathNet.Numerics.LinearAlgebra;
using Throw;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Element1DAggregate;

public partial class Element1DTests
{
    [SkippableTheory]
    [ClassData(typeof(AllElement1DFixtures))]
    public void GetTransformationMatrix_ForAllElement1DFixtures_ShouldEqualExpectedValue(
        AnalyticalElement1DFixture fixture
    )
    {
        _ = fixture.ExpectedTransformationMatrix.ThrowIfNull(() => throw new SkipException());

        Matrix<double> transformationMatrix = fixture.Element.GetTransformationMatrix();

        transformationMatrix.AssertAlmostEqual(fixture.ExpectedTransformationMatrix);
    }

    [SkippableTheory]
    [ClassData(typeof(AllElement1DFixtures))]
    public void GetGlobalStiffnessMatrix_ForAllElement1DFixtures_ShouldEqualExpectedValue(
        AnalyticalElement1DFixture fixture
    )
    {
        _ = fixture.ExpectedGlobalStiffnessMatrix.ThrowIfNull(() => throw new SkipException());

        Matrix<double> globalStiffnessMatrix = fixture
            .Element
            .GetGlobalStiffnessMatrix(
                fixture.UnitSettings.ForceUnit,
                fixture.UnitSettings.ForcePerLengthUnit,
                fixture.UnitSettings.TorqueUnit
            );

        globalStiffnessMatrix.AssertAlmostEqual(fixture.ExpectedGlobalStiffnessMatrix, 1);
    }
}
