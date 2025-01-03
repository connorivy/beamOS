using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.NodeResult;
using BeamOs.Tests.Common;
using FluentAssertions;
using UnitsNet.Units;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public class OpenSeesTests
{
    private static Dictionary<Guid, int> resultSetIdDict = [];

    [Before(HookType.Class)]
    public static async Task RunOpenSeesAnalysis()
    {
        foreach (var modelBuilder in AllSolvedProblems.ModelFixtures())
        {
            await modelBuilder.Build(AssemblySetup.StructuralAnalysisApiClient);
            var resultSetIdResponse = await AssemblySetup
                .StructuralAnalysisApiClient
                .RunOpenSeesAnalysisAsync(modelBuilder.Id);

            if (resultSetIdResponse.IsError)
            {
                throw new Exception(resultSetIdResponse.Error.Description);
            }
            resultSetIdDict[modelBuilder.Id] = resultSetIdResponse.Value;
        }
    }

    [Test]
    [MethodDataSource(typeof(AllSolvedProblems), nameof(AllSolvedProblems.ModelFixtures))]
    public async Task AssertNodeResults_AreApproxEqualToExpectedValues(ModelFixture modelFixture)
    {
        var nodeResultsFixture = (IHasExpectedNodeResults)modelFixture;
        int resultSetId = resultSetIdDict[modelFixture.Id];
        var strongUnits = modelFixture.Settings.UnitSettings.ToDomain();
        foreach (var expectedNodeDisplacementResult in nodeResultsFixture.ExpectedNodeResults)
        {
            if (
                expectedNodeDisplacementResult.DisplacementAlongX.HasValue
                || expectedNodeDisplacementResult.DisplacementAlongY.HasValue
                || expectedNodeDisplacementResult.DisplacementAlongZ.HasValue
                || expectedNodeDisplacementResult.RotationAboutX.HasValue
                || expectedNodeDisplacementResult.RotationAboutY.HasValue
                || expectedNodeDisplacementResult.RotationAboutZ.HasValue
            )
            {
                var result = await AssemblySetup
                    .StructuralAnalysisApiClient
                    .GetNodeResultAsync(
                        modelFixture.Id,
                        resultSetId,
                        expectedNodeDisplacementResult.NodeId
                    );

                AssertDisplacementsEqual(
                    expectedNodeDisplacementResult.NodeId.ToString(),
                    expectedNodeDisplacementResult,
                    result.Value.Displacements,
                    strongUnits.LengthUnit,
                    strongUnits.AngleUnit,
                    .001
                );
            }

            if (
                expectedNodeDisplacementResult.ForceAlongX.HasValue
                || expectedNodeDisplacementResult.ForceAlongY.HasValue
                || expectedNodeDisplacementResult.ForceAlongZ.HasValue
                || expectedNodeDisplacementResult.TorqueAboutX.HasValue
                || expectedNodeDisplacementResult.TorqueAboutY.HasValue
                || expectedNodeDisplacementResult.TorqueAboutZ.HasValue
            )
            {
                var result = await AssemblySetup
                    .StructuralAnalysisApiClient
                    .GetNodeResultAsync(
                        modelFixture.Id,
                        resultSetId,
                        expectedNodeDisplacementResult.NodeId
                    );

                AssertReactionsEqual(
                    expectedNodeDisplacementResult.NodeId.ToString(),
                    expectedNodeDisplacementResult,
                    result.Value.Forces,
                    strongUnits.ForceUnit,
                    strongUnits.TorqueUnit,
                    .01
                );
            }
        }
    }

    private static void AssertDisplacementsEqual(
        string dbId,
        NodeResultFixture expected,
        DisplacementsResponse calculated,
        LengthUnit lengthUnit,
        AngleUnit angleUnit,
        double precision = .001
    )
    {
        Asserter.AssertEqual(
            dbId,
            "Node Displacement",

            [
                expected.DisplacementAlongX?.As(lengthUnit),
                expected.DisplacementAlongY?.As(lengthUnit),
                expected.DisplacementAlongZ?.As(lengthUnit),
                expected.RotationAboutX?.As(angleUnit),
                expected.RotationAboutY?.As(angleUnit),
                expected.RotationAboutZ?.As(angleUnit)
            ],

            [
                calculated.DisplacementAlongX.Value,
                calculated.DisplacementAlongY.Value,
                calculated.DisplacementAlongZ.Value,
                calculated.RotationAboutX.Value,
                calculated.RotationAboutY.Value,
                calculated.RotationAboutZ.Value
            ],
            precision,

            [
                nameof(expected.DisplacementAlongX),
                nameof(expected.DisplacementAlongY),
                nameof(expected.DisplacementAlongZ),
                nameof(expected.RotationAboutX),
                nameof(expected.RotationAboutY),
                nameof(expected.RotationAboutZ),
            ]
        );
    }

    private static void AssertReactionsEqual(
        string dbId,
        NodeResultFixture expected,
        ForcesResponse calculated,
        ForceUnit forceUnit,
        TorqueUnit torqueUnit,
        double precision = .001
    )
    {
        Asserter.AssertEqual(
            dbId,
            "Node Reactions",

            [
                expected.ForceAlongX?.As(forceUnit),
                expected.ForceAlongY?.As(forceUnit),
                expected.ForceAlongZ?.As(forceUnit),
                expected.TorqueAboutX?.As(torqueUnit),
                expected.TorqueAboutY?.As(torqueUnit),
                expected.TorqueAboutZ?.As(torqueUnit)
            ],

            [
                calculated.ForceAlongX.Value,
                calculated.ForceAlongY.Value,
                calculated.ForceAlongZ.Value,
                calculated.MomentAboutX.Value,
                calculated.MomentAboutY.Value,
                calculated.MomentAboutZ.Value
            ],
            precision,

            [
                nameof(calculated.ForceAlongX),
                nameof(calculated.ForceAlongY),
                nameof(calculated.ForceAlongZ),
                nameof(calculated.MomentAboutX),
                nameof(calculated.MomentAboutY),
                nameof(calculated.MomentAboutZ),
            ]
        );
    }
}
