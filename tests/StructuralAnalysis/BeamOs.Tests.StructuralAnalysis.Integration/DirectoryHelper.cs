namespace BeamOs.Tests.StructuralAnalysis.Integration;

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

    public static string GetStructuralAnalysisFunctionsDir() =>
        Path.Combine(
            GetStructuralAnalysisDir(),
            $"{nameof(BeamOs)}.{nameof(BeamOs.StructuralAnalysis)}.{nameof(BeamOs.StructuralAnalysis.Api)}.{nameof(BeamOs.StructuralAnalysis.Api.Functions)}"
        );

    public static string GetTestsDir() => Path.Combine(GetRootDirectory(), "tests");
}
