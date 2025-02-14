using System.Text;

namespace BeamOs.CodeGen.SpeckleConnectorApi;

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
