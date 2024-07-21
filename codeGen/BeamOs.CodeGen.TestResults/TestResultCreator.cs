using System.Diagnostics;
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
    private const string StrykerResultFileName = "index.html";

    private readonly string testResultsPath = Path.Combine(
        DirectoryHelper.GetServerWwwrootDir(),
        PathToTestResults
    );

    public async Task<CodeTestScoreSnapshot> CreateNewDataPointForVersion()
    {
        await this.GenerateTestReports();

        var numTests = GetNumTests();
        var codCov = this.GetCodeCov();
        var mutationScore = await this.GetMutationScore();

        return new CodeTestScoreSnapshot(numTests, codCov, mutationScore);
    }

    public async Task GenerateTestReports()
    {
        Process toolRestore = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = $"tool restore",
                WorkingDirectory = DirectoryHelper.GetRootDirectory()
            }
        };

        toolRestore.Start();
        await toolRestore.WaitForExitAsync();

        Process testProcess = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments =
                    "test -c Release /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:ExcludeByAttribute=\"Obsolete%2cGeneratedCodeAttribute%2cCompilerGeneratedAttribute\"",
                WorkingDirectory = DirectoryHelper.GetRootDirectory()
            }
        };
        testProcess.Start();
        await testProcess.WaitForExitAsync();

        Process codeCovProcess = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments =
                    $"reportgenerator -targetdir:{Path.Combine(this.testResultsPath, CodeCovResultDirName)}",
                WorkingDirectory = DirectoryHelper.GetRootDirectory()
            }
        };

        codeCovProcess.Start();
        await codeCovProcess.WaitForExitAsync();

        Process strykerProcess = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments =
                    $"stryker --output {Path.Combine(this.testResultsPath, StrykerResultDirName)}",
                WorkingDirectory = DirectoryHelper.GetRootDirectory()
            }
        };

        strykerProcess.Start();
        await strykerProcess.WaitForExitAsync();

        this.MoveStrykerOutputToCorrectLocation();
    }

    private void MoveStrykerOutputToCorrectLocation()
    {
        string defaultStrykerOutputFile = Path.Combine(
            this.testResultsPath,
            StrykerResultDirName,
            "reports",
            StrykerResultFileName
        );
        string desiredStrykerOutputFile = Path.Combine(
            this.testResultsPath,
            StrykerResultDirName,
            StrykerResultFileName
        );

        if (File.Exists(desiredStrykerOutputFile))
        {
            File.Delete(desiredStrykerOutputFile);
        }

        File.Move(defaultStrykerOutputFile, desiredStrykerOutputFile);
        Directory.Delete(Path.Combine(this.testResultsPath, StrykerResultDirName, "reports"));
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
