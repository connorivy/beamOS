using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Services;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Interfaces;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems;
using BeamOS.Tests.Common.Traits;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod;

[DirectStiffnessMethod]
public class DsmModelTests
{
    [SkippableTheory]
    [ClassData(typeof(AllSolvedDsmProblems))]
    public void StructuralStiffnessMatrix_ForSampleProblems_ShouldResultInExpectedValues(
        DsmModelFixture modelFixture
    )
    {
        if (modelFixture is not IHasStructuralStiffnessMatrix modelFixtureWithSsm)
        {
            throw new SkipException("No expected value to test against calculated value");
        }

        var unitSettings = modelFixture.Fixture.UnitSettings;
        var nodes = modelFixture.DsmNodeFixtures.Select(modelFixture.ToDsm).ToArray();
        var element1ds = modelFixture.DsmElement1dFixtures.Select(modelFixture.ToDsm).ToArray();
        var (degreeOfFreedomIds, boundaryConditionIds) =
            DirectStiffnessMethodSolver.GetSortedUnsupportedStructureIds(nodes);

        double[,] structureStiffnessMatrix = DirectStiffnessMethodSolver
            .BuildStructureStiffnessMatrix(
                degreeOfFreedomIds,
                element1ds,
                unitSettings.ForceUnit,
                unitSettings.ForcePerLengthUnit,
                unitSettings.TorqueUnit
            )
            .Values;

        int numRows = modelFixtureWithSsm.ExpectedStructuralStiffnessMatrix.GetLength(0);
        int numColumns = modelFixtureWithSsm.ExpectedStructuralStiffnessMatrix.GetLength(1);
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                Assert.Equal(
                    modelFixtureWithSsm.ExpectedStructuralStiffnessMatrix[row, col],
                    structureStiffnessMatrix[row, col],
                    0
                );
            }
        }
    }

    [SkippableTheory]
    [ClassData(typeof(AllSolvedDsmProblems))]
    public void JointDisplacementVector_ForSampleProblem_ShouldResultInExpectedValues(
        DsmModelFixture modelFixture
    )
    {
        if (modelFixture is not IHasExpectedDisplacementVector modelFixtureWithJdv)
        {
            throw new SkipException("No expected value to test against calculated value");
        }

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
        double[] jointDisplacementVector = DirectStiffnessMethodSolver
            .GetUnknownJointDisplacementVector(
                structureStiffnessMatrix,
                knownReactionVector,
                degreeOfFreedomIds
            )
            .Values;

        int numColumns = modelFixtureWithJdv.ExpectedDisplacementVector.Length;
        for (int i = 0; i < numColumns; i++)
        {
            Assert.Equal(
                modelFixtureWithJdv.ExpectedDisplacementVector[i],
                jointDisplacementVector[i],
                2
            );
        }
    }

    [SkippableTheory]
    [ClassData(typeof(AllSolvedDsmProblems))]
    public void JointReactionVector_ForSampleProblem_ShouldResultInExpectedValues(
        DsmModelFixture modelFixture
    )
    {
        if (modelFixture is not IHasExpectedReactionVector modelFixtureWithJrv)
        {
            throw new SkipException("No expected value to test against calculated value");
        }

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
        double[] jointReactionVector = DirectStiffnessMethodSolver
            .GetUnknownJointReactionVector(
                boundaryConditionIds,
                jointDisplacementVector,
                element1ds,
                unitSettings.ForceUnit,
                unitSettings.ForcePerLengthUnit,
                unitSettings.TorqueUnit
            )
            .Values;

        int numColumns = modelFixtureWithJrv.ExpectedReactionVector.Length;
        for (int i = 0; i < numColumns; i++)
        {
            Assert.Equal(modelFixtureWithJrv.ExpectedReactionVector[i], jointReactionVector[i], 2);
        }
    }
}