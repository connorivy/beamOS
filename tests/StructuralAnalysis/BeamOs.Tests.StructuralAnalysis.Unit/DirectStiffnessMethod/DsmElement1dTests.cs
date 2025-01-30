using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.CsSdk.Mappers;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Tests.Common;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod;

public partial class DsmElement1dTests
{
    [Test]
    [MethodDataSource(
        typeof(AllSolvedProblems),
        nameof(AllSolvedProblems.ModelFixturesWithDsmElement1dResults)
    )]
    public void JointReactionVector_ForSampleProblem_ShouldResultInExpectedValues(
        ModelFixture modelFixture
    )
    {
        BeamOsModelBuilderDomainMapper mapper = new(modelFixture.Id);
        var dsmFixture = (IHasDsmElement1dResults)modelFixture;
        DsmAnalysisModel dsmAnalysisModel = mapper.ToDsm(modelFixture, out var model);

        VectorIdentified jointDisplacementVector =
            dsmAnalysisModel.GetUnknownJointDisplacementVector();

        foreach (var el in dsmFixture.ExpectedDsmElement1dResults)
        {
            var physicalEl = model.Element1ds.First(x => x.Id == el.ElementId);

            if (el.ExpectedGlobalEndDisplacements is not null)
            {
                var jointDispVector = new DsmElement1d(physicalEl)
                    .GetGlobalEndDisplacementVector(jointDisplacementVector)
                    .AsArray();

                Asserter.AssertEqual(
                    BeamOsObjectType.Element1d,
                    physicalEl.Id.ToString(),
                    "Global Joint Displacements",
                    el.ExpectedGlobalEndDisplacements,
                    jointDispVector
                );
            }

            if (el.ExpectedGlobalEndForces is not null)
            {
                var jointDispVector = new DsmElement1d(physicalEl)
                    .GetGlobalMemberEndForcesVector(
                        jointDisplacementVector,
                        model.Settings.UnitSettings.ForceUnit,
                        model.Settings.UnitSettings.ForcePerLengthUnit,
                        model.Settings.UnitSettings.TorqueUnit
                    )
                    .AsArray();

                Asserter.AssertEqual(
                    BeamOsObjectType.Element1d,
                    physicalEl.Id.ToString(),
                    "Global Joint Displacements",
                    el.ExpectedGlobalEndForces,
                    jointDispVector,
                    .02
                );
            }
        }
    }
}
