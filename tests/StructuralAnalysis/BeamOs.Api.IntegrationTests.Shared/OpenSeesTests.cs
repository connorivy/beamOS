using BeamOs.ApiClient.Builders;
#if RUNTIME_TESTS
using BeamOs.ApiClient;
#else

#endif

namespace BeamOs.Api.IntegrationTests;

#if RUNTIME_TESTS
public class OpenSeesTests(IApiAlphaClient apiClient) : DirectStiffnessMethodTestsBase(apiClient)
{
#else
public class OpenSeesTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    : DirectStiffnessMethodTestsBase(webApplicationFactory)
{
#endif

    protected override Task RunAnalysis(FixtureId modelId) =>
        this.ApiClient.RunOpenSeesAnalysisAsync(modelId.ToString());
}
