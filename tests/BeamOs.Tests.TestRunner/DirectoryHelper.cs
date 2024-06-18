namespace BeamOs.Tests.TestRunner;

public static class DirectoryHelper
{
    private static readonly string directoryHelperDir = Path.GetDirectoryName(
        typeof(DirectoryHelper).Assembly.Location
    );

    public static string GetRootDirectory()
    {
        DirectoryInfo directory = new(directoryHelperDir);
        while (directory.Name != "beamOS")
        {
            directory = directory.Parent;
        }

        return directory.FullName;
    }

    public static string GetSrcDir() => Path.Combine(GetRootDirectory(), "src");

    public static string GetWebAppDir() =>
        Path.Combine(GetSrcDir(), $"{nameof(BeamOS)}.{nameof(BeamOS.WebApp)}");

    public static string GetWebAppServerDir() =>
        Path.Combine(GetWebAppDir(), $"{nameof(BeamOS)}.{nameof(BeamOS.WebApp)}");

    public static string GetWebAppClientDir() =>
        Path.Combine(GetWebAppDir(), "BeamOS.WebApp.Client");

    public static string GetServerWwwrootDir() => Path.Combine(GetWebAppServerDir(), "wwwroot");

    public static string GetTestsDir() => Path.Combine(GetRootDirectory(), "tests");

    public static string GetTestRunnerDir() =>
        Path.Combine(GetTestsDir(), $"{nameof(BeamOs)}.{nameof(Tests)}.{nameof(Tests.TestRunner)}");
}
