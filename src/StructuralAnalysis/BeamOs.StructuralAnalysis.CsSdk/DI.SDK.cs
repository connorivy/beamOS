using System.Net.Http.Headers;
using System.Text;
using BeamOs.CodeGen.SpeckleConnectorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.CsSdk;

public static class DI
{
    public static IServiceCollection AddBeamOs(this IServiceCollection services, string apiToken)
    {
        services.AddHttpClient();
        AuthMessageHandler authMessageHandler = new(apiToken);
        services.AddSingleton(authMessageHandler);

        services
            .AddHttpClient<IStructuralAnalysisApiClientV1, StructuralAnalysisApiClientV1>(
                client => client.BaseAddress = new("https://api.beamos.net/")
            )
            .AddHttpMessageHandler<AuthMessageHandler>();

        services
            .AddHttpClient<ISpeckleConnectorApi, SpeckleConnectorApi>(
                client => client.BaseAddress = new("https://api.beamos.net/")
            )
            .AddHttpMessageHandler<AuthMessageHandler>();

        return services;
    }
}

public class AuthMessageHandler(string apiToken) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes(apiToken))
        );
        return await base.SendAsync(request, cancellationToken);
    }
}
