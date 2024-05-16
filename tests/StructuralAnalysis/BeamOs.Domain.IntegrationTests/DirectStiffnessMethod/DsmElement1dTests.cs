using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Services;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Mappers;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems;
using BeamOS.Tests.Common.Extensions;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Element1DAggregate;

public partial class DsmElement1dTests
{
    [Theory]
    [ClassData(typeof(AllSolvedDsmProblems))]
    public void JointReactionVector_ForSampleProblem_ShouldResultInExpectedValues(
        DsmModelFixture modelFixture
    )
    {
        var unitSettings = modelFixture.Fixture.UnitSettings;
        var nodes = modelFixture.DsmNodeFixtures.Select(modelFixture.ToDsm).ToArray();
        var element1ds = modelFixture.DsmElement1dFixtures.Select(modelFixture.ToDsm).ToArray();

        var (degreeOfFreedomIds, boundaryConditionIds) =
            DirectStiffnessMethodSolver.GetSortedUnsupportedStructureIds(nodes);
        MatrixIdentified structureStiffnessMatrix =
            DirectStiffnessMethodSolver.BuildStructureStiffnessMatrix(
                degreeOfFreedomIds,
                element1ds,
                unitSettings.ForceUnit,
                unitSettings.ForcePerLengthUnit,
                unitSettings.TorqueUnit
            );
        VectorIdentified knownReactionVector =
            DirectStiffnessMethodSolver.BuildKnownJointReactionVector(
                degreeOfFreedomIds,
                nodes,
                unitSettings.ForceUnit,
                unitSettings.TorqueUnit
            );
        VectorIdentified jointDisplacementVector =
            DirectStiffnessMethodSolver.GetUnknownJointDisplacementVector(
                structureStiffnessMatrix,
                knownReactionVector,
                degreeOfFreedomIds
            );
        VectorIdentified jointReactionVector =
            DirectStiffnessMethodSolver.GetUnknownJointReactionVector(
                boundaryConditionIds,
                jointDisplacementVector,
                element1ds,
                unitSettings.ForceUnit,
                unitSettings.ForcePerLengthUnit,
                unitSettings.TorqueUnit
            );

        foreach (var el in modelFixture.DsmElement1dFixtures)
        {
            if (el.ExpectedGlobalEndDisplacements is null)
            {
                continue;
            }

            el.ToDomainObjectWithLocalIds()
                .GetGlobalEndDisplacementVector(jointDisplacementVector)
                .AsArray()
                .AssertAlmostEqual(el.ExpectedGlobalEndDisplacements);
        }
    }
}
