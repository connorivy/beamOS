using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.NodeResult;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.Tests.Common;
using TUnit.Core.Exceptions;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

[MethodDataSource(
    typeof(AllSolvedProblemsWithMulipleClients),
    nameof(AllSolvedProblemsWithMulipleClients.ModelFixtures)
)]
public partial class DsmTests
{
    private readonly ModelFixture modelFixture;
    private readonly ApiClientKey clientKey;

    public DsmTests(ApiClientKey clientKey, ModelFixture modelFixture)
    {
        this.modelFixture = modelFixture;
        this.clientKey = clientKey;
    }

    private BeamOsResultApiClient ApiClient => field ??= this.clientKey.GetClient();

    private BeamOsApiResultModelId ModelClient =>
        field ??= this.ApiClient.Models[this.modelFixture.Id];

    // [Before(TUnitHookType.Test)]
    // public void BeforeClass()
    // {
    //     // This is a workaround to ensure that the API client is initialized before any tests run.
    //     this.modelClient ??= AssemblySetup.StructuralAnalysisRemoteApiClient.Models[
    //         modelFixture.Id
    //     ];
    // }

    [Test, SkipInFrontEnd]
    public async Task RunDsmAnalysis_ShouldReturnSuccessfulStatusCode()
    {
        await modelFixture.CreateOnly(this.ApiClient);

        var resultSetIdResponse = await this.ModelClient.Analyze.Dsm.RunDirectStiffnessMethodAsync(
            new() { LoadCombinationIds = [2] }
        );

        resultSetIdResponse.ThrowIfError();
    }

    [Test]
    [DependsOn(typeof(DsmTests), nameof(RunDsmAnalysis_ShouldReturnSuccessfulStatusCode))]
    public async Task AssertNodeResults_AreApproxEqualToExpectedValues()
    {
        if (modelFixture is not IHasExpectedNodeResults nodeResultsFixture)
        {
            throw new SkipTestException(
                "Model fixture does not implement IHasExpectedDiagramResults"
            );
        }
        if (modelFixture is ISkipDsmTests)
        {
            throw new SkipTestException("Skipping DSM tests for this model fixture");
        }

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
                var result = await this
                    .ModelClient.Results.LoadCombinations[2]
                    .Nodes[expectedNodeDisplacementResult.NodeId]
                    .GetNodeResultAsync();

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
                var result = await this
                    .ModelClient.Results.LoadCombinations[2]
                    .Nodes[expectedNodeDisplacementResult.NodeId]
                    .GetNodeResultAsync();

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

    [Test]
    [DependsOn(typeof(DsmTests), nameof(RunDsmAnalysis_ShouldReturnSuccessfulStatusCode))]
    public async Task ShearDiagrams_ShouldHaveCorrectMaxAndMinValue()
    {
        if (modelFixture is not IHasExpectedDiagramResults expectedDiagramResults)
        {
            throw new SkipTestException(
                "Model fixture does not implement IHasExpectedDiagramResults"
            );
        }

        var resultSet = await ModelClient.ResultSets[2].GetResultSetAsync();

        resultSet.ThrowIfError();

        var expectedDiagramResultsDict = expectedDiagramResults.ExpectedDiagramResults.ToDictionary(
            x => x.NodeId
        );

        if (expectedDiagramResultsDict.Count == 0)
        {
            throw new Exception("No expected diagram results found");
        }

        var unitSettings = modelFixture.Settings.UnitSettings.ToDomain();
        foreach (var element1dResult in resultSet.Value.Element1dResults)
        {
            if (
                !expectedDiagramResultsDict.TryGetValue(
                    element1dResult.Element1dId,
                    out var expectedDiagramResult
                )
            )
            {
                continue;
            }

            TestUtils.Asserter.AssertEqual(
                BeamOsObjectType.Element1d,
                element1dResult.Element1dId.ToString(),
                "Diagram Results",
                [
                    expectedDiagramResult.MinShear?.As(unitSettings.ForceUnit),
                    expectedDiagramResult.MaxShear?.As(unitSettings.ForceUnit),
                    expectedDiagramResult.MinMoment?.As(unitSettings.TorqueUnit),
                    expectedDiagramResult.MaxMoment?.As(unitSettings.TorqueUnit),
                    expectedDiagramResult.MinDeflection?.As(unitSettings.LengthUnit),
                    expectedDiagramResult.MaxDeflection?.As(unitSettings.LengthUnit),
                ],
                [
                    element1dResult.MinShear.Value,
                    element1dResult.MaxShear.Value,
                    element1dResult.MinMoment.Value,
                    element1dResult.MaxMoment.Value,
                    element1dResult.MinDisplacement.Value,
                    element1dResult.MaxDisplacement.Value,
                ],
                .001,
                ["MinShear", "MaxShear", "MinMoment", "MaxMoment", "MinDeflection", "MaxDeflection"]
            );
        }
    }
}
