using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.CodeGen.StructuralAnalysisApiClient;

public partial class StructuralAnalysisApiClientV1
{
    static partial void UpdateJsonSerializerSettings(
        System.Text.Json.JsonSerializerOptions settings
    ) => BeamOsSerializerOptions.DefaultConfig(settings);

#if DEBUG
    partial void Initialize()
    {
        this.ReadResponseAsString = true;
    }
#endif
}
