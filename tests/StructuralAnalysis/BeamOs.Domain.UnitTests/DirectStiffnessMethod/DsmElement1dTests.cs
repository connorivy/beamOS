using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems;
using BeamOS.Tests.Common;
using BeamOS.Tests.Common.Traits;

namespace BeamOs.Domain.UnitTests.DirectStiffnessMethod;

[DirectStiffnessMethod]
public partial class DsmElement1dTests
{
    [SkippableTheory]
    [ClassData(typeof(AllDsmElement1dFixtures))]
    public void GetTransformationMatrix_ForAllElement1DFixtures_ShouldEqualExpectedValue(
        DsmElement1dFixture fixture
    )
    {
        if (fixture.ExpectedTransformationMatrix is null)
        {
            throw new SkipException("No expected value to test against calculated value");
        }

        double[,] matrix = fixture.ToDomain().GetTransformationMatrix().ToArray();

        Asserter.AssertEqual("Transformation Matrix", fixture.ExpectedTransformationMatrix, matrix);

        Asserter.AssertEqual(
            fixture.Id.ToString(),
            "Transformation Matrix",
            fixture.ExpectedTransformationMatrix,
            matrix
        );
    }
}
