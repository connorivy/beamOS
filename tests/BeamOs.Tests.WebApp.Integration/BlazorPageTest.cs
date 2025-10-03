using System.CodeDom;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Playwright;
using TUnit.Playwright;

namespace BeamOs.Tests.WebApp.Integration;

public class BlazorPageTest<TProgram> : BrowserTest
    where TProgram : class
{
    // private BlazorApplicationFactory<TProgram>? host;
    // private IPage? page;
    // private IBrowserContext? context;

    // public IBrowserContext Context =>
    //     this.context ?? throw new InvalidOperationException("Setup has not been run.");

    // public IPage Page =>
    //     this.page ?? throw new InvalidOperationException("Setup has not been run.");

    // public BlazorApplicationFactory<TProgram> Host =>
    //     this.host ?? throw new InvalidOperationException("Setup has not been run.");

    public virtual BlazorApplicationFactory<TProgram> CreateHostFactory() =>
        new BlazorApplicationFactory<TProgram>(this.ConfigureWebHost);

    public virtual BrowserNewContextOptions ContextOptions() => null!;

    protected virtual void ConfigureWebHost(IWebHostBuilder builder) { }

    public async Task<BlazorApplicationFactory<TProgram>> CreateHostAsync()
    {
        var host = this.CreateHostFactory();
        await host.StartAsync();
        return host;
    }

    public async Task<PlaywrightPageState> CreatePage(
        bool captureState = false,
        [CallerMemberName] string? testName = null
    )
    {
        var host = new BlazorApplicationFactory<TProgram>(this.ConfigureWebHost);
        await host.StartAsync();

        BrowserNewContextOptions options = this.ContextOptions() ?? new();
        options.BaseURL = host.ServerAddress;
        options.IgnoreHTTPSErrors = true;

        var context = await this.NewContext(options);
        var page = await context.NewPageAsync();

        if (captureState)
        {
            await context.Tracing.StartAsync(
                new()
                {
                    Screenshots = true,
                    Snapshots = true,
                    Sources = true,
                    Title = testName,
                }
            );
        }

        return new PlaywrightPageState(this.Browser, context, page, captureState ? testName : null);
    }

    // public async Task InitializeAsync()
    // {
    //     this.host = new BlazorApplicationFactory<TProgram>(this.ConfigureWebHost);
    //     await this.host.StartAsync();

    //     var options = this.ContextOptions() ?? new BrowserNewContextOptions();
    //     options.BaseURL = this.Host.ServerAddress;
    //     options.IgnoreHTTPSErrors = true;

    //     this.context = await this.NewContext(options);
    //     this.page = await this.Context.NewPageAsync();
    // }

    // public async ValueTask DisposeAsync()
    // {
    //     if (this.page is not null)
    //     {
    //         await this.page.CloseAsync();
    //     }

    //     if (this.context is not null)
    //     {
    //         await this.context.DisposeAsync();
    //     }

    //     if (this.host is not null)
    //     {
    //         await this.host.DisposeAsync();
    //         this.host = null;
    //     }
    // }
}

public class PlaywrightPageState(
    IBrowser browser,
    IBrowserContext context,
    IPage page,
    string? testNameForCapture = null
) : IAsyncDisposable
{
    public IBrowser Browser { get; } = browser;
    public IBrowserContext Context { get; } = context;
    public IPage Page { get; } = page;

    public async ValueTask DisposeAsync()
    {
        if (this.Page is not null)
        {
            await this.Page.CloseAsync();
        }

        if (testNameForCapture is not null)
        {
            await this.Context.Tracing.StopAsync(new() { Path = $"{testNameForCapture}.zip" });
            Console.WriteLine($"Trace for {testNameForCapture} saved to {testNameForCapture}.zip");
        }

        if (this.Context is not null)
        {
            await this.Context.DisposeAsync();
        }
    }
}
