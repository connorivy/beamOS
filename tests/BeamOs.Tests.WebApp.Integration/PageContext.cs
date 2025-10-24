using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration;

public sealed class PageContext : IAsyncDisposable
{
    private PageContext(
        IBrowser browser,
        IBrowserContext context,
        IPage page,
        string testName,
        bool saveSnapshotsOnSuccess = false
    )
    {
        this.Browser = browser ?? throw new ArgumentNullException(nameof(browser));
        this.Context = context ?? throw new ArgumentNullException(nameof(context));
        this.Page = page ?? throw new ArgumentNullException(nameof(page));
        this.testName = testName;
        this.saveSnapshotsOnSuccess = saveSnapshotsOnSuccess;
    }

    public IBrowser Browser { get; }
    public IBrowserContext Context { get; }
    public IPage Page { get; }
    private readonly string testName;
    private readonly bool saveSnapshotsOnSuccess;

    public static async Task<PageContext> CreateAsync(
        IBrowser browser,
        IBrowserContext context,
        IPage page,
        string testName,
        bool saveSnapshotsOnSuccess = false
    )
    {
        var contextInstance = new PageContext(
            browser,
            context,
            page,
            testName,
            saveSnapshotsOnSuccess
        );
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
        string? path;
        if (testFailed || this.saveSnapshotsOnSuccess)
        {
            path = $"trace-{this.testName}.zip";
            Console.WriteLine(
                $"Test {TestContext.Current?.TestName} failed. View trace info with this command:\n"
                    + $"    npx playwright show-trace ./bin/Debug/net9.0/{path} --host 0.0.0.0"
            // + $"    pwsh ./bin/Debug/net9.0/playwright.ps1 show-trace ./bin/Debug/net9.0/{path}"
            );
        }
        else
        {
            path = null;
        }
        await this.Context.Tracing.StopAsync(new() { Path = path }).ConfigureAwait(false);
        await this.Page.CloseAsync().ConfigureAwait(false);
        // await this.Context.CloseAsync().ConfigureAwait(false);
        // await this.Browser.CloseAsync().ConfigureAwait(false);
    }
}
