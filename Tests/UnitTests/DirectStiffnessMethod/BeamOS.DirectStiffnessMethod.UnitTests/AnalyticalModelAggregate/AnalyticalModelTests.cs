using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Extensions;
using BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.AnalyticalModels;
using MathNet.Numerics.LinearAlgebra;
using Throw;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.AnalyticalModelAggregate;
public class AnalyticalModelTests
{
    [SkippableTheory]
    [ClassData(typeof(AllAnalyticalModelFixtures))]
    public void JointDisplacementVector_ForSampleProblem_ShouldResultInExpectedValues(AnalyticalModelFixture fixture)
    {
        _ = fixture.ExpectedSupportDisplacementVector.ThrowIfNull(() => throw new SkipException());

        Vector<double> jointDisplacementVector = fixture.AnalyticalModel.JointDisplacementVector.Build();

        jointDisplacementVector.AssertAlmostEqual(fixture.ExpectedSupportDisplacementVector, 2);
    }

    [SkippableTheory]
    [ClassData(typeof(AllAnalyticalModelFixtures))]
    public void JointReactionVector_ForSampleProblem_ShouldResultInExpectedValues(AnalyticalModelFixture fixture)
    {
        _ = fixture.ExpectedSupportReactionVector.ThrowIfNull(() => throw new SkipException());

        Vector<double> jointDisplacementVector = fixture.AnalyticalModel.ReactionVector.Build();

        jointDisplacementVector.AssertAlmostEqual(fixture.ExpectedSupportReactionVector, 2);
    }
}
