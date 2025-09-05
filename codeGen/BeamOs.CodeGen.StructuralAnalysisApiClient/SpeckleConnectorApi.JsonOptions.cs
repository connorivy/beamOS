using System.Text;

namespace BeamOs.CodeGen.SpeckleConnectorApi;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Trimming",
    "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
    Justification = "Not an issue because we are using the source generated json serializer options"
)]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "AOT",
    "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
    Justification = "Not an issue because we are using the source generated json serializer options"
)]
public partial class SpeckleConnectorApi
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
