using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration;

public sealed class PageContext : IAsyncDisposable
{
    private PageContext(
        IBrowser browser,
        IBrowserContext context,
        IPage page,
        string testNameIfFailure
    )
    {
        this.Browser = browser ?? throw new ArgumentNullException(nameof(browser));
        this.Context = context ?? throw new ArgumentNullException(nameof(context));
        this.Page = page ?? throw new ArgumentNullException(nameof(page));
        this.testNameIfFailure = testNameIfFailure;
    }

    public IBrowser Browser { get; }
    public IBrowserContext Context { get; }
    public IPage Page { get; }
    private readonly string testNameIfFailure;

    public static async Task<PageContext> CreateAsync(
        IBrowser browser,
        IBrowserContext context,
        IPage page,
        string testNameIfFailure
    )
    {
        var contextInstance = new PageContext(browser, context, page, testNameIfFailure);
        await contextInstance.Context.Tracing.StartAsync(
            new()
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true,
            }
        );
        return contextInstance;
    }

    public async ValueTask DisposeAsync()
    {
        bool testFailed = TestContext.Current.Result.State == TestState.Failed;
        var path = testFailed ? $"trace-{this.testNameIfFailure}.zip" : null;
        await this.Context.Tracing.StopAsync(new() { Path = path }).ConfigureAwait(false);
        if (testFailed)
        {
            Console.WriteLine(
                $"Test {TestContext.Current?.TestName} failed. View trace info with this command:\n"
                    + $"    npx playwright show-trace ./bin/Debug/net9.0/{path} --host 0.0.0.0"
            // + $"    pwsh ./bin/Debug/net9.0/playwright.ps1 show-trace ./bin/Debug/net9.0/{path}"
            );
        }
        await this.Page.CloseAsync().ConfigureAwait(false);
        // await this.Context.CloseAsync().ConfigureAwait(false);
        // await this.Browser.CloseAsync().ConfigureAwait(false);
    }
}
