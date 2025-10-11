using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Infrastructure;
using BeamOs.Tests.Common;
using BeamOs.Tests.StructuralAnalysis.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using TUnit.Playwright;

namespace BeamOs.Tests.WebApp.Integration;

public class BlazorPageTest<TBlazorApp> : ContextTest
    where TBlazorApp : class
{
    // private static BlazorApplicationFactory<TBlazorApp>? host;

    // [Before(TUnitHookType.Class)]
    // public static async Task BlazorWebAppFactorySetup()
    // {
    //     host = new BlazorApplicationFactory<TBlazorApp>();
    //     await host.StartAsync();
    // }

    public override BrowserNewContextOptions ContextOptions(TestContext testContext)
    {
        var options = base.ContextOptions(testContext);
        options.BaseURL =
            "http://localhost:5077"
            ?? throw new InvalidOperationException("Host is not initialized.");
        return options;
    }

    public PageContext PageContext { get; private set; } = null!;
    public IPage Page => this.PageContext.Page;

    [Before(TUnitHookType.Test, "", 0)]
    public async Task PageSetup()
    {
        if (this.Context == null)
        {
            throw new InvalidOperationException(
                $"Browser context is not initialized. This may indicate that {nameof(ContextTest)}.{nameof(ContextSetup)} did not execute properly."
            );
        }

        this.PageContext = await PageContext
            .CreateAsync(
                this.Browser!,
                this.Context,
                await this.Context.NewPageAsync().ConfigureAwait(false),
                TestContext.Current.TestName
            )
            .ConfigureAwait(false);
    }

    [After(TUnitHookType.Test, "", 0)]
    public async Task PageTeardown()
    {
        if (this.PageContext != null)
        {
            Console.WriteLine($"Disposing PageContext for test {TestContext.Current?.TestName}");
            await this.PageContext.DisposeAsync().ConfigureAwait(false);
            this.PageContext = null!;
        }
    }
}

public class BlazorPageTestWithBackend<TBlazorApp, TApi> : BlazorPageTest<TBlazorApp>
    where TBlazorApp : class
    where TApi : class
{
    protected virtual Action<IServiceCollection>? ConfigureDb { get; } =
        (services) =>
        {
            using IServiceScope scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StructuralAnalysisDbContext>();

            if (BeamOsEnv.IsCiEnv())
            {
                dbContext.Database.Migrate();
            }
            else
            {
                dbContext.Database.EnsureCreated();
            }
        };

    protected virtual Action<IServiceCollection>? ConfigureServices { get; }

    protected WebAppFactoryBase<TApi> Factory { get; private set; }

    protected HttpClient HttpClient { get; private set; }
    protected Uri BaseAddress =>
        this.HttpClient.BaseAddress
        ?? throw new InvalidOperationException("HttpClient BaseAddress is null");

    [Before(TUnitHookType.Test, "", 0)]
    public async Task WebAppFactorySetup()
    {
        this.Factory = new WebAppFactoryBase<TApi>(
            Common.Integration.DbTestContainer.GetConnectionString(),
            services =>
            {
                this.ConfigureServices?.Invoke(services);
                this.ConfigureDb?.Invoke(services);
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
