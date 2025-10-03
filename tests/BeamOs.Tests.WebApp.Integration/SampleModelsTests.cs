using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using NSubstitute;

namespace BeamOs.Tests.WebApp.Integration;

public class SampleModelsTests : BlazorPageTest<BeamOs.WebApp.App>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.ConfigureServices(services =>
        {
            var client = Substitute.For<IStructuralAnalysisApiClientV1>();
            client
                .GetModelsAsync(Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromResult(ApiResponse.FromValue<ICollection<ModelInfoResponse>>([]))
                );
            services.AddSingleton(client);
        });

    [Test]
    public async Task ModelsPage_WhenUserIsUnauthenticated_ShouldShowTwistyBowlFraming()
    {
        var pageState = await this.CreatePage();
        await pageState.Page.GotoAsync("/models", new() { WaitUntil = WaitUntilState.NetworkIdle });

        await this.Expect(
                pageState.Page.GetByRole(AriaRole.Heading, new() { Name = "Twisty Bowl Framing" })
            )
            .ToBeVisibleAsync();
    }
}
