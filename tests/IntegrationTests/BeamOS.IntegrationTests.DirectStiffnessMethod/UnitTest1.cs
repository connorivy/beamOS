using BeamOs.ApiClient;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Services;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Interfaces;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems.MatrixAnalysisOfStructures2ndEd.Example8_4;
using BeamOS.Tests.Common.SolvedProblems;
using BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using FluentAssertions;
using Xunit.Sdk;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod;

public class UnitTest1(CustomWebApplicationFactory<BeamOs.Api.Program> webApplicationFactory)
    : IClassFixture<CustomWebApplicationFactory<BeamOs.Api.Program>>
{
    //[Fact]
    //public async Task Test1()
    //{
    //    var httpClient = webApplicationFactory.CreateClient();
    //    var client = new ApiAlphaClient(httpClient);
    //    await Example8_4f.CreatePhysicalModel(client);

    //    var modelResponse = await client.GetModelAsync(Example8_4f.ModelId, null);
    //    var expectedModelResponse = Example8_4f.GetExpectedResponse();

    //    ContractComparer.AssertContractsEqual(modelResponse, expectedModelResponse);
    //}

    //[Fact]
    //public async Task Test2()
    //{
    //    var httpClient = webApplicationFactory.CreateClient();
    //    var client = new ApiAlphaClient(httpClient);
    //    var modelFixture = new Example8_4f();

    //    await modelFixture.Create(client);

    //    var expectedModelResponse = modelFixture.GetExpectedResponse();
    //    var modelResponse = await client.GetModelAsync(modelFixture.Id.ToString(), null);

    //    ContractComparer.AssertContractsEqual(modelResponse, expectedModelResponse);
    //}

    //[Fact]
    //public async Task Test3()
    //{
    //    var httpClient = webApplicationFactory.CreateClient();
    //    var client = new ApiAlphaClient(httpClient);
    //    var modelFixture = new Example8_4f();

    //    await modelFixture.Create(client);
    //    await client.RunDirectStiffnessMethodAsync(new IdRequest(modelFixture.ModelId));

    //    var expectedModelResponse = modelFixture.GetExpectedNodeResultFixtures();
    //    var modelResponse = await client.GetModelAsync(modelFixture.Id.ToString(), null);

    //    ICollection<NodeResultResponse> calculatedNodeResponses = await client.GetNodeResultsAsync(
    //        modelFixture.ModelId,
    //        []
    //    );
    //    //foreach (var x in modelFixture.ExpectedNodeResults)
    //    //{
    //    //    NodeResponse? calculatedResult = await client.GetNodeResultsAsync(modelFixture.ModelId, );

    //    //}

    //    ContractComparer.AssertContractsEqual(
    //        expectedModelResponse,
    //        calculatedNodeResponses.ToArray()
    //    );
    //}

    [Theory]
    [ClassData(typeof(AllSolvedDsmProblems))]
    public void Test_Dsm_StructuralStiffnessMatrix(DsmModelFixture modelFixture)
    {
        if (modelFixture is not IHasStructuralStiffnessMatrix modelFixtureWithSsm)
        {
            throw SkipException.ForSkip("No expected value to test against calculated value");
        }
        //var httpClient = webApplicationFactory.CreateClient();
        //var client = new ApiAlphaClient(httpClient);
        //var baseFixture = new Kassimali_Example8_4();

        ////await baseFixture.Create(client);
        ////await client.RunDirectStiffnessMethodAsync(new IdRequest(modelFixture.Fixture.ModelId));

        //var modelFixture = new Example8_4_Dsm(baseFixture);
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

        int numRows = modelFixtureWithSsm.ExpectedStructuralStiffnessMatrix.GetLength(0);
        int numColumns = modelFixtureWithSsm.ExpectedStructuralStiffnessMatrix.GetLength(1);
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                Assert.Equal(
                    modelFixtureWithSsm.ExpectedStructuralStiffnessMatrix[row, col],
                    structureStiffnessMatrix.Values[row, col],
                    0
                );
            }
        }
    }

    [Theory]
    [ClassData(typeof(AllDsmElement1dFixtures))]
    public void Test_Dsm_GlobalStiffnessMatrix(DsmElement1dFixture element1dFixture)
    {
        if (element1dFixture.ExpectedGlobalStiffnessMatrix is null)
        {
            throw SkipException.ForSkip("No expected value to test against calculated value");
        }

        var dsmEl = element1dFixture.ToDomainObjectWithLocalIds();
        var actualRotationMatrix = dsmEl.GetRotationMatrix().ToArray();
        actualRotationMatrix
            .Should()
            .BeEquivalentTo(
                element1dFixture.ExpectedRotationMatrix,
                options =>
                    options
                        .ComparingByValue<double>()
                        .Using<double>(
                            ctx =>
                                ctx.Subject.Should().BeApproximately(ctx.Expectation, precision: 2)
                        )
                        .WhenTypeIs<double>()
            );
    }

    //[Fact]
    //public async Task Test_Dsm_StructuralStiffnessMatrix()
    //{
    //    var httpClient = webApplicationFactory.CreateClient();
    //    var client = new ApiAlphaClient(httpClient);
    //    var baseFixture = new Example8_4f();

    //    await baseFixture.Create(client);
    //    //await client.RunDirectStiffnessMethodAsync(new IdRequest(modelFixture.Fixture.ModelId));

    //    var modelFixture = new Example8_4f_Dsm(baseFixture);
    //    var unitSettings = modelFixture.Fixture.UnitSettings;
    //    var nodes = modelFixture.DsmNodes;
    //    var element1ds = modelFixture.DsmElement1ds;
    //    var (degreeOfFreedomIds, boundaryConditionIds) =
    //        DirectStiffnessMethodSolver.GetSortedUnsupportedStructureIds(nodes);

    //    MatrixIdentified structureStiffnessMatrix =
    //        DirectStiffnessMethodSolver.BuildStructureStiffnessMatrix(
    //            degreeOfFreedomIds,
    //            element1ds,
    //            unitSettings.ForceUnit,
    //            unitSettings.ForcePerLengthUnit,
    //            unitSettings.TorqueUnit
    //        );

    //    int numRows = modelFixture.ExpectedStructuralStiffnessMatrix.GetLength(0);
    //    int numColumns = modelFixture.ExpectedStructuralStiffnessMatrix.GetLength(1);
    //    for (int row = 0; row < numRows; row++)
    //    {
    //        for (int col = 0; col < numColumns; col++)
    //        {
    //            Assert.Equal(
    //                modelFixture.ExpectedStructuralStiffnessMatrix[row, col],
    //                structureStiffnessMatrix.Values[row, col],
    //                3
    //            );
    //        }
    //    }
    //}
}
