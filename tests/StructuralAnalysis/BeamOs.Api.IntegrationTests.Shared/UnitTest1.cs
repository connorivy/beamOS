using BeamOs.ApiClient;
using BeamOs.ApiClient.Builders;
using BeamOs.Application.Common.Mappers;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOS.Tests.Common;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Fixtures.Mappers.ToDomain;
using BeamOS.Tests.Common.SolvedProblems;
using UnitsNet.Units;
using Xunit;
#if RUNTIME_TESTS
using BeamOs.Api.IntegrationTests.Runtime;
#else

#endif

namespace BeamOs.Api.IntegrationTests;

#if RUNTIME_TESTS
public class UnitTest1 : IAsyncLifetime
{
    public UnitTest1(IApiAlphaClient apiClient)
    {
        this.apiClient = apiClient;
    }
#else
public class UnitTest1 : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    public UnitTest1(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        var httpClient = webApplicationFactory.CreateClient();
        this.apiClient = new ApiAlphaClient(httpClient);
    }
#endif

    private readonly IApiAlphaClient apiClient;
    private readonly Dictionary<string, IModelFixtureInDb> modelIdToModelFixtureDict = [];

    public async Task InitializeAsync()
    {
        //AllModelFixtures allSolved = new();
        //foreach (var fixture in allSolved.GetItems())
        //{
        //    var dbModelFixture = new ModelFixtureInDb(fixture);
        //    await dbModelFixture.Create(this.apiClient);
        //    this.modelIdToModelFixtureDict.Add(fixture.Id.ToString(), dbModelFixture);
        //    await this.apiClient.RunDirectStiffnessMethodAsync(fixture.Id.ToString());
        //}

        AllCreateModelRequestBuilders allModelBuilders = new();
        foreach (var modelBuilder in allModelBuilders.GetItems())
        {
            if (!modelBuilder.IsInitialized)
            {
                ModelResponse? modelResponse;
                try
                {
                    modelResponse = await this.apiClient.GetModelAsync(
                        modelBuilder.Id.ToString(),

                        [
                            nameof(Model.Nodes),
                            nameof(Model.Element1ds),
                            $"{nameof(Model.Element1ds)}.{nameof(Element1D.SectionProfile)}",
                            $"{nameof(Model.Element1ds)}.{nameof(Element1D.Material)}",
                            $"{nameof(Model.Nodes)}.{nameof(Node.PointLoads)}",
                            $"{nameof(Model.Nodes)}.{nameof(Node.MomentLoads)}"
                        ]
                    );
                    await this.apiClient.RunDirectStiffnessMethodAsync(modelBuilder.Id.ToString());
                }
                catch (Exception ex)
                {
                    modelResponse = null;
                }
                if (modelResponse is not null)
                {
                    modelBuilder.InitializeFromModelResponse(modelResponse);
                }
                else
                {
                    await modelBuilder.InitializeAsync();
                    await modelBuilder.Create(this.apiClient);
                    await this.apiClient.RunDirectStiffnessMethodAsync(modelBuilder.Id.ToString());
                }
                modelBuilder.IsInitialized = true;
            }
            this.modelIdToModelFixtureDict.Add(modelBuilder.Id.ToString(), modelBuilder);
        }
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    //[SkippableTheory]
    //[ClassData(typeof(AllModelFixtures))]
    //public async Task ModelResponses_WhenRetrievedFromDb_ShouldEqualExpectedValues(
    //    ModelFixture2 modelFixture
    //)
    //{
    //    var dbModelFixture = this.modelIdToModelFixtureDict[modelFixture.Id.ToString()];

    //    var modelResponse = await this.apiClient.GetModelAsync(modelFixture.Id.ToString(), null);
    //    var expectedModelResponse = dbModelFixture.ToResponse();

    //    ContractComparer.AssertContractsEqual(modelResponse, expectedModelResponse);
    //}

    [BeamOsTheory]
    [ClassData(typeof(AllCreateModelRequestBuildersFilter<IHasExpectedNodeDisplacementResults>))]
    public async Task NodeDisplacementResults_ForSampleProblems_ShouldResultInExpectedValues(
        IHasExpectedNodeDisplacementResults modelFixture
    )
    {
        var dbModelFixture = this.modelIdToModelFixtureDict[modelFixture.Id.ToString()];

        var strongUnits = modelFixture.Settings.UnitSettings.ToDomain();
        foreach (var expectedNodeDisplacementResult in modelFixture.ExpectedNodeDisplacementResults)
        {
            var dbId = dbModelFixture.RuntimeIdToDbId(expectedNodeDisplacementResult.NodeId);
            NodeResultResponse? result = null;

            if (
                expectedNodeDisplacementResult.DisplacementAlongX.HasValue
                || expectedNodeDisplacementResult.DisplacementAlongY.HasValue
                || expectedNodeDisplacementResult.DisplacementAlongZ.HasValue
                || expectedNodeDisplacementResult.RotationAboutX.HasValue
                || expectedNodeDisplacementResult.RotationAboutY.HasValue
                || expectedNodeDisplacementResult.RotationAboutZ.HasValue
            )
            {
                // for now we don't support multiple results for a node (i.e. load combinations)
                // so just take the first result
                result ??= (await this.apiClient.GetSingleNodeResultAsync(dbId)).First();
                AssertQuantitiesEqual(
                    dbId,
                    expectedNodeDisplacementResult,
                    result,
                    strongUnits.LengthUnit,
                    strongUnits.AngleUnit,
                    2
                );
            }

            //if (
            //    expectedNodeDisplacementResult.ForceAlongX.HasValue
            //    || expectedNodeDisplacementResult.ForceAlongY.HasValue
            //    || expectedNodeDisplacementResult.ForceAlongZ.HasValue
            //    || expectedNodeDisplacementResult.TorqueAboutX.HasValue
            //    || expectedNodeDisplacementResult.TorqueAboutY.HasValue
            //    || expectedNodeDisplacementResult.TorqueAboutZ.HasValue
            //)
            //{
            //    result ??= (await this.apiClient.GetSingleNodeResultAsync(dbId)).First();
            //    AssertQuantitiesEqual(
            //        dbId,
            //        expectedNodeDisplacementResult,
            //        result,
            //        forceUnit,
            //        torqueUnit
            //    );
            //}
        }
    }

    private static void AssertQuantitiesEqual(
        string dbId,
        NodeResultFixture expected,
        NodeResultResponse calculated,
        LengthUnit lengthUnit,
        AngleUnit angleUnit,
        int numDigits = 3
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
                calculated.Displacements.DisplacementAlongX.Value,
                calculated.Displacements.DisplacementAlongY.Value,
                calculated.Displacements.DisplacementAlongZ.Value,
                calculated.Displacements.RotationAboutX.Value,
                calculated.Displacements.RotationAboutY.Value,
                calculated.Displacements.RotationAboutZ.Value
            ],
            numDigits,

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

    private static void AssertQuantitiesEqual(
        string dbId,
        NodeResultFixture expected,
        NodeResultResponse calculated,
        ForceUnit forceUnit,
        TorqueUnit torqueUnit,
        int numDigits = 3
    )
    {
        Asserter.AssertEqual(
            dbId,
            "Node Forces",

            [
                expected.ForceAlongX?.As(forceUnit),
                expected.ForceAlongY?.As(forceUnit),
                expected.ForceAlongZ?.As(forceUnit),
                expected.TorqueAboutX?.As(torqueUnit),
                expected.TorqueAboutY?.As(torqueUnit),
                expected.TorqueAboutZ?.As(torqueUnit)
            ],

            [
                calculated.Forces.ForceAlongX.Value,
                calculated.Forces.ForceAlongY.Value,
                calculated.Forces.ForceAlongZ.Value,
                calculated.Forces.MomentAboutX.Value,
                calculated.Forces.MomentAboutY.Value,
                calculated.Forces.MomentAboutZ.Value
            ],
            numDigits,

            [
                nameof(expected.ForceAlongX),
                nameof(expected.ForceAlongY),
                nameof(expected.ForceAlongZ),
                nameof(calculated.Forces.MomentAboutX),
                nameof(calculated.Forces.MomentAboutY),
                nameof(calculated.Forces.MomentAboutZ),
            ]
        );
    }
}
