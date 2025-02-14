namespace BeamOs.Tests.Common;

public static class BeamOsEnv
{
    public static bool IsCiEnv()
    {
        return bool.TryParse(
                Environment.GetEnvironmentVariable("ContinuousIntegrationEnv"),
                out bool isCiEnv
            ) && isCiEnv;
    }
}
