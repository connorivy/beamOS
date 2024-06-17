using BeamOs.Tests.TestRunner;
using HtmlAgilityPack;
using PuppeteerSharp;

namespace BeamOs.DevOps.PipelineHelper;

internal class TestResultCreator
{
    private const string PathToTestResults = "test-results";
    private const string CodeCovResultDirName = "code-coverage";
    private const string CodeCovResultFileName = "index.html";
    private const string StrykerResultDirName = "stryker";
    private const string StrykerResultFileName = "mutation-report.html";

    private readonly string testResultsPath = Path.Combine(
        DirectoryHelper.GetServerWwwrootDir(),
        PathToTestResults
    );

    private static Version GetVersion()
    {
        return typeof(BeamOS.WebApp.Program).Assembly.GetName().Version
            ?? throw new Exception("Could not get version");
    }

    public async Task<(Version, CodeTestScoreSnapshot)> CreateNewDataPointForVersion()
    {
        var numTests = GetNumTests();
        var codCov = this.GetCodeCov();
        var mutationScore = await this.GetMutationScore();

        return (GetVersion(), new CodeTestScoreSnapshot(numTests, codCov, mutationScore));
    }

    private float GetCodeCov()
    {
        HtmlDocument codeCovDoc = new();
        codeCovDoc.Load(
            Path.Combine(this.testResultsPath, CodeCovResultDirName, CodeCovResultFileName)
        );
        var cardNodes = codeCovDoc
            .DocumentNode
            .SelectNodes(
                "html/body/div/div[@class='containerleft']/div[@class='card-group']/div[@class='card']"
            );

        HtmlNode? lineCoverageDoc = null;
        foreach (var card in cardNodes)
        {
            if (card.SelectSingleNode("div[@class='card-header']").InnerHtml == "Line coverage")
            {
                lineCoverageDoc = card;
                break;
            }
        }

        string codCovAsString =
            lineCoverageDoc
                ?.SelectNodes("div[@class='card-body']/div[@class='table']/table/tr")
                .Last()
                .SelectSingleNode("td")
                .InnerHtml
                ?[..^1] ?? throw new Exception("could not find line coverage card html element");

        return float.Parse(codCovAsString);
    }

    private async Task<float> GetMutationScore()
    {
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(
            new LaunchOptions { Headless = true }
        );
        await using var page = await browser.NewPageAsync();
        await page.GoToAsync(
            $"file://{Path.Combine(this.testResultsPath, StrykerResultDirName, StrykerResultFileName)}"
        );

        var mutationScoreAsString = await page.QuerySelectorAsync(
                "xpath/html/body/mutation-test-report-app"
            )
            .QuerySelectorAsyncFluent("pierce/#mte-mutant-view")
            .QuerySelectorAsyncFluent("pierce/main mte-metrics-table")
            .QuerySelectorAsyncFluent(
                "pierce/div > table > tbody > tr:nth-child(1) > td:nth-child(3) > span"
            )
            .GetInnerTextAsync();

        return float.Parse(mutationScoreAsString);
    }

    private static int GetNumTests()
    {
        return AssemblyScanning.GetAllTestInfo().Count();
    }
}

public static class IElementExtensions
{
    public static async Task<IElementHandle> QuerySelectorAsyncFluent(
        this Task<IElementHandle> task,
        string selector
    )
    {
        return await (await task).QuerySelectorAsync(selector);
    }

    public static async Task<string> GetInnerTextAsync(this IElementHandle elementHandle)
    {
        return await elementHandle.EvaluateFunctionAsync<string>("e => e.innerText");
    }

    public static async Task<string> GetInnerTextAsync(this Task<IElementHandle> elementTask)
    {
        return await (await elementTask).EvaluateFunctionAsync<string>("e => e.innerText");
    }
}
