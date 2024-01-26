using FastEndpoints.ClientGen;
using Microsoft.AspNetCore.Builder;

namespace BeamOS.Common.Api;

public static class ApiClientGenerator
{
    public const string BeamOsNs = nameof(BeamOS);
    public const string PhysicalModelNs = nameof(BeamOS.PhysicalModel);

    //public const string AnalyticalModelNs = nameof(BeamOS.A);
    public const string ContractsNs = nameof(BeamOS.PhysicalModel.Contracts);
    public const string NodeNs = nameof(BeamOS.PhysicalModel.Contracts.Node);
    public const string Element1dNs = nameof(BeamOS.PhysicalModel.Contracts.Element1D);
    public const string ModelNs = nameof(BeamOS.PhysicalModel.Contracts.Model);
    public const string PointLoadNs = nameof(BeamOS.PhysicalModel.Contracts.PointLoad);
    public const string MomentLoadNs = nameof(BeamOS.PhysicalModel.Contracts.MomentLoad);
    public const string MaterialNs = nameof(BeamOS.PhysicalModel.Contracts.Material);
    public const string SectionProfileNs = nameof(BeamOS.PhysicalModel.Contracts.SectionProfile);

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
