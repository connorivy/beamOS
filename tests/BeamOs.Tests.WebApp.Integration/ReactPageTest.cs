using Microsoft.Playwright;
using TUnit.Playwright;

namespace BeamOs.Tests.WebApp.Integration;

public class ReactPageTest : WebContextTest
{
    public ReactPageTest()
        : base(new() { Timeout = System.Diagnostics.Debugger.IsAttached ? 0 : 15_000 }) { }

    public PageContext PageContext { get; private set; } = null!;
    public IPage Page => this.PageContext.Page;

    public override BrowserNewContextOptions ContextOptions(TestContext testContext)
    {
        var options = base.ContextOptions(testContext);
        // options.BaseURL = this.factory.ServerAddress;
        options.BaseURL = this.FrontendAddressOverride ?? AssemblySetup.FrontendAddress;
        options.IgnoreHTTPSErrors = true;
        return options;
    }

    protected virtual string? FrontendAddressOverride { get; }

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
