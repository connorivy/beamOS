using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Mappers;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems;
using BeamOs.Domain.UnitTests.DirectStiffnessMethod.Common.Extensions;

namespace BeamOs.Domain.UnitTests.DirectStiffnessMethod;

public partial class DsmElement1dTests
{
    [Theory]
    [ClassData(typeof(AllDsmElement1dFixtures))]
    public void GetTransformationMatrix_ForAllElement1DFixtures_ShouldEqualExpectedValue(
        DsmElement1dFixture fixture
    )
    {
        if (fixture.ExpectedTransformationMatrix is null)
        {
            throw new SkipException("No expected value to test against calculated value");
        }

        double[,] matrix = fixture.ToDomainObjectWithLocalIds().GetTransformationMatrix().ToArray();

        matrix.AssertAlmostEqual(fixture.ExpectedTransformationMatrix);
    }
}
