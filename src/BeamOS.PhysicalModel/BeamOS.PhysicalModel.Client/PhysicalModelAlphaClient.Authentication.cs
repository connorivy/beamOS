namespace BeamOS.PhysicalModel.Client;

public partial class PhysicalModelAlphaClient
{
    protected virtual void PrepareRequestCustom(
        HttpClient client,
        HttpRequestMessage request,
        string url
    ) { }

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
    {
        this.PrepareRequestCustom(client, request, url);
    }
}
