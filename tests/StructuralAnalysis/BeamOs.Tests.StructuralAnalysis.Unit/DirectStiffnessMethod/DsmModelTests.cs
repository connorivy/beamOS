using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.CsSdk.Mappers;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.Tests.Common;

namespace BeamOs.Tests.StructuralAnalysis.Unit.DirectStiffnessMethod;

public class DsmModelTests
{
    [Test]
    [MethodDataSource(
        typeof(AllSolvedProblems),
        nameof(AllSolvedProblems.ModelFixturesWithStructuralStiffnessMatrix)
    )]
    public void StructuralStiffnessMatrix_ForSampleProblems_ShouldResultInExpectedValues(
        ModelFixture modelFixture
    )
    {
        BeamOsModelBuilderDomainMapper mapper = new(modelFixture.Id);
        var dsmFixture = (IHasStructuralStiffnessMatrix)modelFixture;
        DsmAnalysisModel dsmAnalysisModel = mapper.ToDsm(modelFixture, out _);

        double[,] structureStiffnessMatrix = dsmAnalysisModel
            .BuildStructureStiffnessMatrix()
            .Values;

        Asserter.AssertEqual(
            BeamOsObjectType.Model,
            modelFixture.Id.ToString(),
            "Structural Stiffness Matrix",
            dsmFixture.ExpectedStructuralStiffnessMatrix,
            structureStiffnessMatrix,
            .5
        );
    }

    [Test]
    [MethodDataSource(
        typeof(AllSolvedProblems),
        nameof(AllSolvedProblems.ModelFixturesWithExpectedDisplacementVector)
    )]
    public void JointDisplacementVector_ForSampleProblem_ShouldResultInExpectedValues(
        ModelFixture modelFixture
    )
    {
        BeamOsModelBuilderDomainMapper mapper = new(modelFixture.Id);
        var dsmFixture = (IHasExpectedDisplacementVector)modelFixture;
        DsmAnalysisModel dsmAnalysisModel = mapper.ToDsm(modelFixture, out _);

        double[] jointDisplacementVector = dsmAnalysisModel
            .GetUnknownJointDisplacementVector()
            .Values;

        Asserter.AssertEqual(
            BeamOsObjectType.Model,
            modelFixture.Id.ToString(),
            "Joint Displacement Vector",
            dsmFixture.ExpectedDisplacementVector,
            jointDisplacementVector,
            .00001
        );
    }

    [Test]
    [MethodDataSource(
        typeof(AllSolvedProblems),
        nameof(AllSolvedProblems.ModelFixturesWithExpectedReactionVector)
    )]
    public void JointReactionVector_ForSampleProblem_ShouldResultInExpectedValues(
        ModelFixture modelFixture
    )
    {
        BeamOsModelBuilderDomainMapper mapper = new(modelFixture.Id);
        var dsmFixture = (IHasExpectedReactionVector)modelFixture;
        DsmAnalysisModel dsmAnalysisModel = mapper.ToDsm(modelFixture, out _);

        double[] jointReactionVector = dsmAnalysisModel.GetUnknownJointReactionVector().Values;

        Asserter.AssertEqual(
            BeamOsObjectType.Model,
            modelFixture.Id.ToString(),
            "Joint Reaction Vector",
            dsmFixture.ExpectedReactionVector,
            jointReactionVector,
            .005
        );
    }
}
