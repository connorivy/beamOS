using BeamOs.Domain.DirectStiffnessMethod;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Interfaces;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems;
using BeamOS.Tests.Common;
using BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers.ToDomain;
using BeamOS.Tests.Common.Traits;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod;

[DirectStiffnessMethod]
public class DsmModelTests
{
    [SkippableTheory]
    [ClassData(typeof(AllSolvedDsmProblems))]
    public void StructuralStiffnessMatrix_ForSampleProblems_ShouldResultInExpectedValues(
        IDsmModelFixture modelFixture
    )
    {
        if (modelFixture is not IHasStructuralStiffnessMatrix modelFixtureWithSsm)
        {
            throw new SkipException("No expected value to test against calculated value");
        }

        DsmAnalysisModel dsmAnalysisModel = modelFixture.ToDomain();

        var (degreeOfFreedomIds, boundaryConditionIds) =
            dsmAnalysisModel.GetSortedUnsupportedStructureIds();

        double[,] structureStiffnessMatrix = dsmAnalysisModel
            .BuildStructureStiffnessMatrix(degreeOfFreedomIds)
            .Values;

        Asserter.AssertEqual(
            "Structural Stiffness Matrix",
            modelFixtureWithSsm.ExpectedStructuralStiffnessMatrix,
            structureStiffnessMatrix,
            0
        );
    }

    [SkippableTheory]
    [ClassData(typeof(AllSolvedDsmProblems))]
    public void JointDisplacementVector_ForSampleProblem_ShouldResultInExpectedValues(
        IDsmModelFixture modelFixture
    )
    {
        if (modelFixture is not IHasExpectedDisplacementVector modelFixtureWithJdv)
        {
            throw new SkipException("No expected value to test against calculated value");
        }

        DsmAnalysisModel dsmAnalysisModel = modelFixture.ToDomain();

        var (degreeOfFreedomIds, boundaryConditionIds) =
            dsmAnalysisModel.GetSortedUnsupportedStructureIds();
        MatrixIdentified structureStiffnessMatrix = dsmAnalysisModel.BuildStructureStiffnessMatrix(
            degreeOfFreedomIds
        );
        VectorIdentified knownReactionVector = dsmAnalysisModel.BuildKnownJointReactionVector(
            degreeOfFreedomIds
        );
        double[] jointDisplacementVector = dsmAnalysisModel
            .GetUnknownJointDisplacementVector(
                structureStiffnessMatrix,
                knownReactionVector,
                degreeOfFreedomIds
            )
            .Values;

        Asserter.AssertEqual(
            "Joint Displacement Vector",
            modelFixtureWithJdv.ExpectedDisplacementVector,
            jointDisplacementVector,
            2
        );
    }

    [SkippableTheory]
    [ClassData(typeof(AllSolvedDsmProblems))]
    public void JointReactionVector_ForSampleProblem_ShouldResultInExpectedValues(
        IDsmModelFixture modelFixture
    )
    {
        if (modelFixture is not IHasExpectedReactionVector modelFixtureWithJrv)
        {
            throw new SkipException("No expected value to test against calculated value");
        }

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
        double[] jointReactionVector = dsmAnalysisModel
            .GetUnknownJointReactionVector(boundaryConditionIds, jointDisplacementVector)
            .Values;

        Asserter.AssertEqual(
            "Joint Reaction Vector",
            modelFixtureWithJrv.ExpectedReactionVector,
            jointReactionVector,
            2
        );
    }
}
