using BeamOs.Domain.DirectStiffnessMethod;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Interfaces;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems;
using BeamOS.Tests.Common.Extensions;
using BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers.ToDomain;
using BeamOS.Tests.Common.Traits;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod;

[DirectStiffnessMethod]
public partial class DsmElement1dTests
{
    [Theory]
    [ClassData(typeof(AllSolvedDsmProblems))]
    public void JointReactionVector_ForSampleProblem_ShouldResultInExpectedValues(
        IDsmModelFixture modelFixture
    )
    {
        DsmAnalysisModel dsmAnalysisModel = modelFixture.ToDomain();

        var (degreeOfFreedomIds, boundaryConditionIds) =
            dsmAnalysisModel.GetSortedUnsupportedStructureIds();
        MatrixIdentified structureStiffnessMatrix = dsmAnalysisModel.BuildStructureStiffnessMatrix(
            degreeOfFreedomIds
        );
        VectorIdentified knownReactionVector = dsmAnalysisModel.BuildKnownJointReactionVector(
            degreeOfFreedomIds
        );
        VectorIdentified jointDisplacementVector =
            dsmAnalysisModel.GetUnknownJointDisplacementVector(
                structureStiffnessMatrix,
                knownReactionVector,
                degreeOfFreedomIds
            );
        VectorIdentified jointReactionVector = dsmAnalysisModel.GetUnknownJointReactionVector(
            boundaryConditionIds,
            jointDisplacementVector
        );

        foreach (var el in modelFixture.DsmElement1dFixtures)
        {
            if (el.ExpectedGlobalEndDisplacements is null)
            {
                continue;
            }

            el.ToDomain()
                .GetGlobalEndDisplacementVector(jointDisplacementVector)
                .AsArray()
                .AssertAlmostEqual(el.ExpectedGlobalEndDisplacements);
        }
    }
}
