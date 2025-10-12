using Microsoft.AspNetCore.Hosting;
using Microsoft.Playwright;
using TUnit.Playwright;

namespace BeamOs.Tests.WebApp.Integration;

public class WebContextTest(BrowserTypeLaunchOptions options) : BrowserTest(options)
{
    public WebContextTest()
        : this(new()) { }

    public IBrowserContext Context { get; private set; } = null!;

    public virtual BrowserNewContextOptions ContextOptions(TestContext testContext)
    {
        return new() { Locale = "en-US", ColorScheme = ColorScheme.Light };
    }

    [Before(TUnitHookType.Test, "", 0)]
    public async Task ContextSetup(TestContext testContext)
    {
        if (Browser == null)
        {
            throw new InvalidOperationException(
                $"Browser is not initialized. This may indicate that {nameof(BrowserTest)}.{nameof(BrowserSetup)} did not execute properly."
            );
        }
        await this.BeforeContextCreate();

        Context = await NewContext(ContextOptions(testContext)).ConfigureAwait(false);
    }

    protected virtual Task BeforeContextCreate() => Task.CompletedTask;
}

public class BlazorPageTestBase<TBlazorApp> : WebContextTest
    where TBlazorApp : class
{
    public BlazorPageTestBase()
        : base(
            new()
            {
                Timeout = 15_000, // 15 seconds
            }
        ) { }

    public PageContext PageContext { get; private set; } = null!;
    public IPage Page => this.PageContext.Page;

    // private BlazorApplicationFactory<TBlazorApp> factory;

    // protected override async Task BeforeContextCreate()
    // {
    //     this.factory = new BlazorApplicationFactory<TBlazorApp>(this.ConfigureWebHost);
    //     await this.factory.StartAsync();
    // }

    protected virtual void ConfigureWebHost(IWebHostBuilder builder) { }

    public override BrowserNewContextOptions ContextOptions(TestContext testContext)
    {
        var options = base.ContextOptions(testContext);
        // options.BaseURL = this.factory.ServerAddress;
        options.BaseURL = AssemblySetup.WebAppFactory.ServerAddress;
        options.IgnoreHTTPSErrors = true;
        return options;
    }

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

    // public void Dispose() => this.factory.Dispose();
}

public class BlazorPageTest : BlazorPageTestBase<BeamOs.WebApp._Imports> { }
