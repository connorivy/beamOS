using BeamOs.ApiClient;
using BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOs.Api.IntegrationTests;

public class UnitTest1(CustomWebApplicationFactory<Program> webApplicationFactory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    [Fact]
    public async Task Test1()
    {
        var httpClient = webApplicationFactory.CreateClient();
        var client = new ApiAlphaClient(httpClient);
        var problem = new Kassimali_Example8_4();
        var dbModelFixture = new ModelFixtureInDb(problem);

        await dbModelFixture.Create(client);

        var modelResponse = await client.GetModelAsync(dbModelFixture.ModelFixture.ModelId, null);
        var expectedModelResponse = dbModelFixture.GetExpectedResponse();

        ContractComparer.AssertContractsEqual(modelResponse, expectedModelResponse);
    }
}
