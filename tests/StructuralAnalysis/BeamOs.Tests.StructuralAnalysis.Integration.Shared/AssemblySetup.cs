using BeamOs.StructuralAnalysis.Sdk;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public static partial class AssemblySetup
{
    public static BeamOsResultApiClient StructuralAnalysisApiClient { get; set; }

    public static Func<BeamOsResultApiClient> GetStructuralAnalysisApiClientV1() =>
        () =>
        {
            return StructuralAnalysisApiClient;
        };
}
