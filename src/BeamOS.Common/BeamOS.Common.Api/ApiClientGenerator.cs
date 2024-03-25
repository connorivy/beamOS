using FastEndpoints.ClientGen;
using Microsoft.AspNetCore.Builder;

namespace BeamOS.Common.Api;

public static class ApiClientGenerator
{
    public static async Task GenerateClient(
        this WebApplication app,
        string releaseName,
        string clientNamespace,
        string clientName,
        string[] additionalNamespaces,
        string? pathToClient = null
    )
    {
        await app.GenerateClientsAndExitAsync(
            releaseName,
            pathToClient ?? $"../{clientNamespace}/",
            csSettings: c =>
            {
                c.ClassName = clientName;
                c.GenerateDtoTypes = false;
                c.AdditionalNamespaceUsages = additionalNamespaces;
                c.GenerateClientInterfaces = true;
                c.CSharpGeneratorSettings.Namespace = clientNamespace;
                c.UseBaseUrl = false;
            },
            tsSettings: t =>
            {
                t.ClassName = clientName;
                t.GenerateClientInterfaces = true;
                t.TypeScriptGeneratorSettings.Namespace = ""; // needed to not generate a namespace
            }
        );
    }
}
