using BeamOs.ApiClient;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.SolvedProblems.MatrixAnalysisOfStructures2ndEd;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod;

public class UnitTest1(CustomWebApplicationFactory<Program> webApplicationFactory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    [Fact]
    public async Task Test1()
    {
        var httpClient = webApplicationFactory.CreateClient();
        var client = new ApiAlphaClient(httpClient);
        await Example8_4.CreatePhysicalModel(client);

        var modelResponse = await client.GetModelHydratedAsync(Example8_4.ModelId);
        var expectedModelResponse = Example8_4.GetExpectedResponse();

        ContractComparer.AssertContractsEqual(modelResponse, expectedModelResponse);
    }
}
