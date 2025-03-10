namespace BeamOs.Tests.Common;

public static class BeamOsEnv
{
    public static bool IsCiEnv() => GetBoolFromEnvVar("ContinuousIntegrationEnv");

    public static bool IsPreviewEnv() => GetBoolFromEnvVar("BEAMOS_PREVIEW");

    private static bool GetBoolFromEnvVar(string var) =>
        bool.TryParse(Environment.GetEnvironmentVariable(var), out bool envVarAsBool)
        && envVarAsBool;
}
