using BeamOs.Tests.TestRunner;

namespace BeamOs.DevOps.PipelineHelper;

/// <summary>
/// This program will generate the code coverage and mutation reports in the 'wwwroot' folder of the server project
/// This program is currently only able to run in release mode, because building in debug mode messes with running stryker
/// however, stryker will soon support running in release configuration, so hopefully this will flip
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        CodeTestScoresTracker codeTestScoresTracker = new();
        TestResultCreator testResultCreator = new();
        Version currentVersion = GetVersion();

        KeyValuePair<Version, CodeTestScoreSnapshot>? lastRecord = codeTestScoresTracker
            .CodeTestScores
            .LastOrDefault();

        //if (IsCiBuild())
        //{
        //    if (lastRecord.HasValue && currentVersion > lastRecord.Value.Key)
        //    {
        //        throw new Exception("Code test scores were not added for this version of beamOS");
        //    }

        //    CodeTestScoreSnapshot testResult =
        //        await testResultCreator.CreateNewDataPointForVersion();
        //    if (lastRecord.HasValue && !TestResultsAlmostEqual(lastRecord.Value.Value, testResult))
        //    {
        //        throw new Exception("Actual test result values differ from recorded values");
        //    }
        //}
        //else
        //{

        CodeTestScoreSnapshot testResult = await testResultCreator.CreateNewDataPointForVersion();

        if (!NewTestScoreIsHigherThanPreviousOne(lastRecord.Value.Value, testResult))
        {
            throw new Exception(
                $"Test scores fell! Unacceptable! \n\nPrevious scores: \n{lastRecord.Value.Value} \n\nCurrent scores: \n{testResult}"
            );
        }

        codeTestScoresTracker.AddScore(currentVersion, testResult);
        codeTestScoresTracker.Save();
    }

    private static Version GetVersion()
    {
        return typeof(BeamOs.Tests.TestRunner.AssemblyScanning).Assembly.GetName().Version
            ?? throw new Exception("Could not get version");
    }

    private static bool IsCiBuild()
    {
        return bool.TryParse(
                Environment.GetEnvironmentVariable("ContinuousIntegrationBuild"),
                out bool isCiBuild
            ) && isCiBuild;
    }

    private static bool TestResultsAlmostEqual(
        CodeTestScoreSnapshot expected,
        CodeTestScoreSnapshot actual
    )
    {
        return expected.NumTests == actual.NumTests
            && Math.Abs(expected.CodeCoveragePercent - actual.CodeCoveragePercent) < .001
            && Math.Abs(expected.MutationScore - actual.MutationScore) < .8; // mutation score can vary a good bit between runs
    }

    private static bool NewTestScoreIsHigherThanPreviousOne(
        CodeTestScoreSnapshot previous,
        CodeTestScoreSnapshot newer
    )
    {
        return newer.NumTests >= previous.NumTests
            && (newer.CodeCoveragePercent >= previous.CodeCoveragePercent)
            && (newer.MutationScore >= (previous.MutationScore - .8)); // mutation score can vary a good bit between runs
    }
}
