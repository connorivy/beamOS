using FastEndpoints.ClientGen;
using Microsoft.AspNetCore.Builder;

namespace BeamOs.Api.Common;

public static class ApiClientGenerator
{
    public const string BeamOsNs = nameof(BeamOs);
    public const string PhysicalModelNs = nameof(BeamOs.Contracts.PhysicalModel);
    public const string AnalyticalResultsNs = nameof(BeamOs.Contracts.AnalyticalResults);

    //public const string AnalyticalModelNs = nameof(BeamOs.A);
    public const string ContractsNs = nameof(BeamOs.Contracts);
    public const string NodeNs = nameof(BeamOs.Contracts.PhysicalModel.Node);
    public const string Element1dNs = nameof(BeamOs.Contracts.PhysicalModel.Element1d);
    public const string ModelNs = nameof(BeamOs.Contracts.PhysicalModel.Model);
    public const string PointLoadNs = nameof(BeamOs.Contracts.PhysicalModel.PointLoad);
    public const string MomentLoadNs = nameof(BeamOs.Contracts.PhysicalModel.MomentLoad);
    public const string MaterialNs = nameof(BeamOs.Contracts.PhysicalModel.Material);
    public const string SectionProfileNs = nameof(BeamOs.Contracts.PhysicalModel.SectionProfile);
    public const string AnalyticalModelNs = nameof(BeamOs.Contracts.AnalyticalResults.Model);

    public static async Task GenerateClient(
        this WebApplication app,
        string releaseName,
        string clientNamespace,
        string clientName,
        string[] additionalNamespaces,
        string? pathToClient = null
    )
    {
        //System.Diagnostics.Debugger.Launch();
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
