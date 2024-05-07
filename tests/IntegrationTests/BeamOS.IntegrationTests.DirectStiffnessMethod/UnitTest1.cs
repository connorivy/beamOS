using BeamOs.ApiClient;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.SolvedProblems.MatrixAnalysisOfStructures2ndEd;
using BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;

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

        var expectedModelResponse = modelFixture.GetExpectedResponse();
        var modelResponse = await client.GetModelAsync(modelFixture.Id.ToString(), null);

        foreach (var x in modelFixture.ExpectedNodeResults)
        {
            NodeResponse? calculatedResult = await client.GetSingleNodeAsync("");
        }

        ContractComparer.AssertContractsEqual(modelResponse, expectedModelResponse);
    }
}
