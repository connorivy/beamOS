using System.Text;

namespace BeamOs.CodeGen.StructuralAnalysisApiClient;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Trimming",
    "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
    Justification = "Okay to supress because we are using the source generated jsonSerializerOptions"
)]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "AOT",
    "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
    Justification = "Okay to supress because we are using the source generated jsonSerializerOptions"
)]
public partial class StructuralAnalysisApiClientV1
{
    static partial void UpdateJsonSerializerSettings(
        System.Text.Json.JsonSerializerOptions settings
    ) => BeamOsSerializerOptions.DefaultConfig(settings);

    partial void PrepareRequest(
        HttpClient client,
        HttpRequestMessage request,
        StringBuilder urlBuilder
    ) => this.PrepareRequestProtected(client, request, urlBuilder);

    protected virtual void PrepareRequestProtected(
        HttpClient client,
        HttpRequestMessage request,
        StringBuilder urlBuilder
    ) { }

#if DEBUG
    partial void Initialize()
    {
        this.ReadResponseAsString = true;
    }
#endif
}
