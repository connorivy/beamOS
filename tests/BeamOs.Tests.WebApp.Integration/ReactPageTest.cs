using System;
using System.Collections.Concurrent;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Infrastructure;
using BeamOs.Tests.Common;
using BeamOs.Tests.StructuralAnalysis.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Playwright;

namespace BeamOs.Tests.WebApp.Integration;

public class ReactPageTest : ContextTest
{
    private static ConcurrentBag<string> testNames = new();
    public PageContext PageContext { get; private set; } = null!;

    [Before(TUnitHookType.Test, "", 0)]
    public async Task PageSetup()
    {
        if (this.Context == null)
        {
            throw new InvalidOperationException(
                $"Browser context is not initialized. This may indicate that {nameof(ContextTest)}.{nameof(ContextSetup)} did not execute properly."
            );
        }

        var testName = TestContext.Current.TestName;
        string? testNameIfFailure = null;
        if (testNames.Contains(testName))
        {
            testNameIfFailure = testName;
        }
        else
        {
            testNames.Add(testName);
        }

        this.PageContext = await PageContext
            .CreateAsync(
                this.Browser!,
                this.Context,
                await this.Context.NewPageAsync().ConfigureAwait(false),
                testNameIfFailure
            )
            .ConfigureAwait(false);
    }

    protected string GetUrl(string relativeUrl) => "http://localhost:5173" + relativeUrl;
}

public class ReactPageTestWithBackend : ReactPageTest
{
    protected virtual Action<IServiceCollection>? ConfigureServices { get; }

    protected WebAppFactoryBase<IAssemblyMarkerStructuralAnalysisApi> Factory { get; private set; }

    private HttpClient HttpClient { get; set; }
    protected Uri BaseAddress =>
        this.HttpClient.BaseAddress
        ?? throw new InvalidOperationException("HttpClient BaseAddress is null");

    [Before(TUnitHookType.Test, "", 0)]
    public async Task WebAppFactorySetup()
    {
        this.Factory = new WebAppFactoryBase<IAssemblyMarkerStructuralAnalysisApi>(
            Common.Integration.DbTestContainer.GetConnectionString(),
            services =>
            {
                using IServiceScope scope = services.BuildServiceProvider().CreateScope();
                var dbContext =
                    scope.ServiceProvider.GetRequiredService<StructuralAnalysisDbContext>();

                if (BeamOsEnv.IsCiEnv())
                {
                    dbContext.Database.Migrate();
                }
                else
                {
                    dbContext.Database.EnsureCreated();
                }

                this.ConfigureServices?.Invoke(services);
            }
        );
        this.HttpClient = this.Factory.CreateClient();
        await this.PageContext.Page.AddInitScriptAsync(
            $"window.BASE_API_URL = '{this.HttpClient.BaseAddress}';"
        );
    }

    protected BeamOsResultApiClient ApiClient =>
        new(StructuralAnalysisApiClientV2.CreateFromHttpClient(this.HttpClient));
}
