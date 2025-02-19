using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.NodeResult;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.Tests.Common;
using UnitsNet.Units;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

[ParallelGroup("DsmTests")]
public sealed class DsmTests
{
    [Before(HookType.Class)]
    public static async Task RunDsmAnalysis()
    {
        foreach (var modelBuilder in AllSolvedProblems.ModelFixtures())
        {
            await modelBuilder.InitializeAsync();

            await modelBuilder.CreateOnly(AssemblySetup.StructuralAnalysisApiClient);

            await AssemblySetup
                .StructuralAnalysisApiClient
                .DeleteAllResultSetsAsync(modelBuilder.Id);

            var resultSetIdResponse = await AssemblySetup
                .StructuralAnalysisApiClient
                .RunDirectStiffnessMethodAsync(modelBuilder.Id);

            resultSetIdResponse.ThrowIfError();
        }
    }

    [Test]
    [MethodDataSource(
        typeof(AllSolvedProblems),
        nameof(AllSolvedProblems.ModelFixturesWithExpectedNodeResults)
    )]
    public async Task AssertNodeResults_AreApproxEqualToExpectedValues(IModelFixture modelFixture)
    {
        await modelFixture.InitializeAsync();
        var nodeResultsFixture = (IHasExpectedNodeResults)modelFixture;
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
                        expectedNodeDisplacementResult.ResultSetId,
                        expectedNodeDisplacementResult.NodeId
                    );

                result.ThrowIfError();

                AssertDisplacementsEqual(
                    BeamOsObjectType.Node,
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
                        expectedNodeDisplacementResult.ResultSetId,
                        expectedNodeDisplacementResult.NodeId
                    );

                result.ThrowIfError();

                AssertReactionsEqual(
                    BeamOsObjectType.Node,
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
        BeamOsObjectType beamOsObjectType,
        string dbId,
        NodeResultFixture expected,
        DisplacementsResponse calculated,
        LengthUnit lengthUnit,
        AngleUnit angleUnit,
        double precision = .001
    )
    {
        Asserter.AssertEqual(
            beamOsObjectType,
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
        BeamOsObjectType beamOsObjectType,
        string dbId,
        NodeResultFixture expected,
        ForcesResponse calculated,
        ForceUnit forceUnit,
        TorqueUnit torqueUnit,
        double precision = .001
    )
    {
        Asserter.AssertEqual(
            beamOsObjectType,
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
