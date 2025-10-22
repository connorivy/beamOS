using Microsoft.AspNetCore.Hosting;
using Microsoft.Playwright;
using TUnit.Playwright;

namespace BeamOs.Tests.WebApp.Integration;

public class WebContextTest(BrowserTypeLaunchOptions options) : BrowserTest(options)
{
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
