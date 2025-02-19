namespace BeamOs.CodeGen.TestModelBuilderGenerator;

public static class DirectoryHelper
{
    private static readonly string directoryHelperDir = Path.GetDirectoryName(
        typeof(DirectoryHelper).Assembly.Location
    );

    private static string? rootDir;

    public static string GetRootDirectory()
    {
        if (rootDir == null)
        {
            DirectoryInfo directory = new(directoryHelperDir);
            while (!directory.Name.Equals("beamOS", StringComparison.OrdinalIgnoreCase))
            {
                directory = directory.Parent;
            }

            rootDir = directory.FullName;
        }
        return rootDir;
    }

    public static string GetSrcDir() => Path.Combine(GetRootDirectory(), "src");

    public static string GetStructuralAnalysisDir() =>
        Path.Combine(GetSrcDir(), $"{nameof(BeamOs.StructuralAnalysis)}");

#if DEBUG
    const string configuration = "Debug";
#else
    const string configuration = "Release";
#endif
#if NET9_0
    const string targetFramework = "net9.0";
#endif

    public static string GetTestsDir() => Path.Combine(GetRootDirectory(), "tests");

    public static string GetTestsCommonProjectDir() =>
        Path.Combine(GetTestsDir(), "BeamOs.Tests.Common");

    public static string GetSolvedProblemsDir() =>
        Path.Combine(GetTestsCommonProjectDir(), "SolvedProblems");

    public static string GetSAP2000ProblemsDir() => Path.Combine(GetSolvedProblemsDir(), "SAP2000");
}
