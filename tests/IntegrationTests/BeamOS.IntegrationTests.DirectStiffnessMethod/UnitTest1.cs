using BeamOs.ApiClient;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Contracts.Common;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Services;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.SolvedProblems.MatrixAnalysisOfStructures2ndEd;
using BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using FluentAssertions;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod;

public class UnitTest1(CustomWebApplicationFactory<BeamOs.Api.Program> webApplicationFactory)
    : IClassFixture<CustomWebApplicationFactory<BeamOs.Api.Program>>
{
    [Fact]
    public async Task Test1()
    {
        var httpClient = webApplicationFactory.CreateClient();
        var client = new ApiAlphaClient(httpClient);
        await Example8_4.CreatePhysicalModel(client);

        var modelResponse = await client.GetModelAsync(Example8_4.ModelId, null);
        var expectedModelResponse = Example8_4.GetExpectedResponse();

        ContractComparer.AssertContractsEqual(modelResponse, expectedModelResponse);
    }

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

    [Fact]
    public async Task Test3()
    {
        var httpClient = webApplicationFactory.CreateClient();
        var client = new ApiAlphaClient(httpClient);
        var modelFixture = new Example8_4f();

        await modelFixture.Create(client);
        await client.RunDirectStiffnessMethodAsync(new IdRequest(modelFixture.ModelId));

        var expectedModelResponse = modelFixture.GetExpectedNodeResultFixtures();
        var modelResponse = await client.GetModelAsync(modelFixture.Id.ToString(), null);

        ICollection<NodeResultResponse> calculatedNodeResponses = await client.GetNodeResultsAsync(
            modelFixture.ModelId,
            []
        );
        //foreach (var x in modelFixture.ExpectedNodeResults)
        //{
        //    NodeResponse? calculatedResult = await client.GetNodeResultsAsync(modelFixture.ModelId, );

        //}

        ContractComparer.AssertContractsEqual(
            expectedModelResponse,
            calculatedNodeResponses.ToArray()
        );
    }

    [Fact]
    public async Task Test_Dsm_StructuralStiffnessMatrix()
    {
        var httpClient = webApplicationFactory.CreateClient();
        var client = new ApiAlphaClient(httpClient);
        var baseFixture = new Example8_4f();

        await baseFixture.Create(client);
        //await client.RunDirectStiffnessMethodAsync(new IdRequest(modelFixture.Fixture.ModelId));

        var modelFixture = new Example8_4f_Dsm(baseFixture);
        var unitSettings = modelFixture.Fixture.UnitSettings;
        var nodes = modelFixture.DsmNodes;
        var element1ds = modelFixture.DsmElement1ds;
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

        int numRows = modelFixture.ExpectedStructuralStiffnessMatrix.GetLength(0);
        int numColumns = modelFixture.ExpectedStructuralStiffnessMatrix.GetLength(1);
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                Assert.Equal(
                    modelFixture.ExpectedStructuralStiffnessMatrix[row, col],
                    structureStiffnessMatrix.Values[row, col],
                    0
                );
            }
        }
    }

    [Fact]
    public async Task Test_Dsm_GlobalStiffnessMatrix()
    {
        var httpClient = webApplicationFactory.CreateClient();
        var client = new ApiAlphaClient(httpClient);
        var baseFixture = new Example8_4f();

        await baseFixture.Create(client);
        //await client.RunDirectStiffnessMethodAsync(new IdRequest(modelFixture.Fixture.ModelId));

        var modelFixture = new Example8_4f_Dsm(baseFixture);

        foreach (var el in modelFixture.DsmElement1ds2)
        {
            var dsmEl = modelFixture.ToDsm(el);
            var actualRotationMatrix = dsmEl.GetRotationMatrix().ToArray();
            //Assert.Equal(actualRotationMatrix, el.ExpectedRotationMatrix);
            actualRotationMatrix
                .Should()
                .BeEquivalentTo(
                    el.ExpectedRotationMatrix,
                    options =>
                        options
                            .ComparingByValue<double>()
                            .Using<double>(
                                ctx =>
                                    ctx.Subject
                                        .Should()
                                        .BeApproximately(ctx.Expectation, precision: 2)
                            )
                            .WhenTypeIs<double>()
                );
        }
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
