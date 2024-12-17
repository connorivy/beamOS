using BeamOs.CsSdk.Builders;
#if RUNTIME_TESTS
using BeamOs.ApiClient;
#else

#endif

namespace BeamOs.Api.IntegrationTests;

#if RUNTIME_TESTS
public class DirectStiffnessMethodTests(IApiAlphaClient apiClient)
    : DirectStiffnessMethodTestsBase(apiClient)
{
#else
public class DirectStiffnessMethodTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    : DirectStiffnessMethodTestsBase(webApplicationFactory)
{
#endif

    protected override Task RunAnalysis(FixtureId modelId) =>
        this.ApiClient.RunDirectStiffnessMethodAsync(modelId.ToString(), new());
}
