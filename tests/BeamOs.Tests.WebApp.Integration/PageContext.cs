using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration;

public class PageContext : IAsyncDisposable
{
    private PageContext(
        IBrowser browser,
        IBrowserContext context,
        IPage page,
        string? testNameIfFailure
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
    private readonly string? testNameIfFailure;

    public static async Task<PageContext> CreateAsync(
        IBrowser browser,
        IBrowserContext context,
        IPage page,
        string? testNameIfFailure
    )
    {
        var contextInstance = new PageContext(browser, context, page, testNameIfFailure);
        if (testNameIfFailure is not null)
        {
            await contextInstance.Context.Tracing.StartAsync(
                new()
                {
                    Screenshots = true,
                    Snapshots = true,
                    Sources = true,
                }
            );
        }
        return contextInstance;
    }

    public async ValueTask DisposeAsync()
    {
        if (this.testNameIfFailure != null)
        {
            await this
                .Context.Tracing.StopAsync(new() { Path = $"trace-{testNameIfFailure}.zip" })
                .ConfigureAwait(false);
        }
        await this.Page.CloseAsync().ConfigureAwait(false);
        await this.Context.CloseAsync().ConfigureAwait(false);
        await this.Browser.CloseAsync().ConfigureAwait(false);
    }
}
