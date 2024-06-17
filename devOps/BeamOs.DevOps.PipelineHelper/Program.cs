using System.Diagnostics;
using System.IO;
using HtmlAgilityPack;

namespace BeamOs.DevOps.PipelineHelper;

public class Program
{
    public static async Task Main(string[] args)
    {
        CodeTestScoresTracker codeTestScoresTracker = new();

        TestResultCreator testResultCreator = new();
        await testResultCreator.CreateNewDataPointForVersion();
    }

    private static Version GetVersion()
    {
        return typeof(BeamOS.WebApp.Program).Assembly.GetName().Version
            ?? throw new Exception("Could not get version");
    }

    private static bool IsCiBuild()
    {
        return bool.TryParse(
                Environment.GetEnvironmentVariable("ContinuousIntegrationBuild"),
                out bool isCiBuild
            ) && isCiBuild;
    }

    private static void CreateNewDataPointForVersion(
        Version version,
        CodeTestScoresTracker codeTestScoresTracker
    ) { }

    private static void CheckIfDataPointWasCreated(
        Version currentVersion,
        CodeTestScoresTracker codeTestScoresTracker
    ) { }
}
