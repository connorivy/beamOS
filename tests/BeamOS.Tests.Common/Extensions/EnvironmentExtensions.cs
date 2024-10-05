namespace BeamOS.Tests.Common.Extensions;

public static class BeamOsEnvironment
{
    public static bool IsCi() =>
        bool.TryParse(
            Environment.GetEnvironmentVariable("ContinuousIntegrationBuild"),
            out bool isCiBuild
        ) && isCiBuild;
}
