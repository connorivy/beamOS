using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.NodeResult;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.Tests.Common;
using BeamOs.Tests.StructuralAnalysis.Integration;
using TUnit.Core.Attributes;

namespace BeamOs.Tests.Aot;

[MethodDataSource(
    typeof(AllSolvedProblems),
    nameof(AllSolvedProblems.ModelFixturesWithExpectedNodeResults)
)]
public class OpenSeesTests(ModelFixture modelFixture)
{
    private BeamOsResultApiClient client;
    private BeamOsApiResultModelId modelClient;

    [Before(TUnitHookType.Test)]
    public void BeforeClass()
    {
        this.client = ApiClientFactory.CreateResultLocal();
        this.modelClient ??= this.client.Models[modelFixture.Id];
    }

    [Test, SkipInFrontEnd]
    public async Task RunOpenSeesAnalysis_ShouldReturnSuccessfulStatusCode()
    {
        await modelFixture.CreateOnly(this.client);

        var resultSetIdResponse = await this.modelClient.Analyze.Opensees.RunOpenSeesAnalysisAsync(
            new() { LoadCombinationIds = [1] }
        );

        resultSetIdResponse.ThrowIfError();
    }

    [Test]
    [DependsOn(typeof(OpenSeesTests), nameof(RunOpenSeesAnalysis_ShouldReturnSuccessfulStatusCode))]
    public async Task AssertNodeResults_AreApproxEqualToExpectedValues()
    {
        var nodeResultsFixture = (IHasExpectedNodeResults)modelFixture;
        var strongUnits = modelFixture.Settings.UnitSettings;
        var lengthUnit = strongUnits.LengthUnit.MapEnumToLengthUnit();
        var angleUnit = strongUnits.AngleUnit.MapEnumToAngleUnit();
        var forceUnit = strongUnits.ForceUnit.MapEnumToForceUnit();
        var torqueUnit = strongUnits.TorqueUnit.MapEnumToTorqueUnit();
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
                var result = await modelClient
                    .Results.LoadCombinations[1]
                    .Nodes[expectedNodeDisplacementResult.NodeId]
                    .GetNodeResultAsync();

                result.ThrowIfError();

                AssertDisplacementsEqual(
                    BeamOsObjectType.Node,
                    expectedNodeDisplacementResult.NodeId.ToString(),
                    expectedNodeDisplacementResult,
                    result.Value.Displacements,
                    lengthUnit,
                    angleUnit,
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
                var result = await modelClient
                    .Results.LoadCombinations[1]
                    .Nodes[expectedNodeDisplacementResult.NodeId]
                    .GetNodeResultAsync();

                AssertReactionsEqual(
                    BeamOsObjectType.Node,
                    expectedNodeDisplacementResult.NodeId.ToString(),
                    expectedNodeDisplacementResult,
                    result.Value.Forces,
                    forceUnit,
                    torqueUnit,
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
        TestUtils.Asserter.AssertEqual(
            beamOsObjectType,
            dbId,
            "Node Displacement",
            [
                expected.DisplacementAlongX?.As(lengthUnit),
                expected.DisplacementAlongY?.As(lengthUnit),
                expected.DisplacementAlongZ?.As(lengthUnit),
                expected.RotationAboutX?.As(angleUnit),
                expected.RotationAboutY?.As(angleUnit),
                expected.RotationAboutZ?.As(angleUnit),
            ],
            [
                calculated.DisplacementAlongX.Value,
                calculated.DisplacementAlongY.Value,
                calculated.DisplacementAlongZ.Value,
                calculated.RotationAboutX.Value,
                calculated.RotationAboutY.Value,
                calculated.RotationAboutZ.Value,
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
        TestUtils.Asserter.AssertEqual(
            beamOsObjectType,
            dbId,
            "Node Reactions",
            [
                expected.ForceAlongX?.As(forceUnit),
                expected.ForceAlongY?.As(forceUnit),
                expected.ForceAlongZ?.As(forceUnit),
                expected.TorqueAboutX?.As(torqueUnit),
                expected.TorqueAboutY?.As(torqueUnit),
                expected.TorqueAboutZ?.As(torqueUnit),
            ],
            [
                calculated.ForceAlongX.Value,
                calculated.ForceAlongY.Value,
                calculated.ForceAlongZ.Value,
                calculated.MomentAboutX.Value,
                calculated.MomentAboutY.Value,
                calculated.MomentAboutZ.Value,
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

    public static object Create(ModelFixture modelFixture) => new OpenSeesTests(modelFixture);
}
