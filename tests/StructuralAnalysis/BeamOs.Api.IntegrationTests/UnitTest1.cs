using BeamOs.ApiClient;
using BeamOS.Tests.Common.SolvedProblems;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOs.Api.IntegrationTests;

public class UnitTest1(CustomWebApplicationFactory<Program> webApplicationFactory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    [SkippableTheory]
    [ClassData(typeof(AllSolvedProblems))]
    public async Task Test1(ModelFixture modelFixture)
    {
        var httpClient = webApplicationFactory.CreateClient();
        var client = new ApiAlphaClient(httpClient);
        var dbModelFixture = new ModelFixtureInDb(modelFixture);

        await dbModelFixture.Create(client);

        var modelResponse = await client.GetModelAsync(
            dbModelFixture.ModelFixture.Id.ToString(),
            null
        );
        var expectedModelResponse = dbModelFixture.GetExpectedResponse();

        ContractComparer.AssertContractsEqual(modelResponse, expectedModelResponse);
    }
}
