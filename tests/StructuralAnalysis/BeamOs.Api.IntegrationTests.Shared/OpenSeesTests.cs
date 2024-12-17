using BeamOs.CsSdk.Builders;
using BeamOS.Tests.Common.Extensions;
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

    protected override Task RunAnalysis(FixtureId modelId)
    {
        if (BeamOsEnvironment.IsCi())
        {
            throw new Xunit.SkipException(
                "Currently unable to run opensees tests in ci environment"
            );
        }
        return this.ApiClient.RunOpenSeesAnalysisAsync(modelId.ToString(), new());
    }
}
