using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Extensions;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalModels;
using MathNet.Numerics.LinearAlgebra;
using Throw;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.AnalyticalModelAggregate;

public class AnalyticalModelTests
{
    [SkippableTheory]
    [ClassData(typeof(AllAnalyticalModelFixtures))]
    public void StructureStiffnessMatrix_ForSampleProblems_ShouldResultInExpectedValues(
        AnalyticalModelFixture fixture
    )
    {
        _ = fixture.ExpectedStructureStiffnessMatrix.ThrowIfNull(() => throw new SkipException());

        Matrix<double> structureStiffnessMatrix = fixture
            .AnalyticalModel
            .StructureStiffnessMatrix
            .Build();

        structureStiffnessMatrix.AssertAlmostEqual(
            fixture.ExpectedStructureStiffnessMatrix,
            fixture.NumberOfDecimalsToCompareSMatrix
        );
    }

    [SkippableTheory]
    [ClassData(typeof(AllAnalyticalModelFixtures))]
    public void JointDisplacementVector_ForSampleProblem_ShouldResultInExpectedValues(
        AnalyticalModelFixture fixture
    )
    {
        _ = fixture.ExpectedDisplacementVector.ThrowIfNull(() => throw new SkipException());

        Vector<double> jointDisplacementVector = fixture
            .AnalyticalModel
            .JointDisplacementVector
            .Build();

        jointDisplacementVector.AssertAlmostEqual(
            fixture.ExpectedDisplacementVector,
            fixture.NumberOfDecimalsToCompareDisplacementVector
        );
    }

    [SkippableTheory]
    [ClassData(typeof(AllAnalyticalModelFixtures))]
    public void JointReactionVector_ForSampleProblem_ShouldResultInExpectedValues(
        AnalyticalModelFixture fixture
    )
    {
        _ = fixture.ExpectedReactionVector.ThrowIfNull(() => throw new SkipException());

        Vector<double> jointDisplacementVector = fixture.AnalyticalModel.ReactionVector.Build();

        jointDisplacementVector.AssertAlmostEqual(
            fixture.ExpectedReactionVector,
            fixture.NumberOfDecimalsToCompareReactionVector
        );
    }
}
