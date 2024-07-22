using FastEndpoints.ClientGen;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.TypeScript;
using NSwag.Generation;

namespace BeamOs.Api.Common;

public static class ApiClientGenerator
{
    public const string BeamOsNs = nameof(BeamOs);
    public const string PhysicalModelNs = nameof(BeamOs.Contracts.PhysicalModel);
    public const string AnalyticalResultsNs = nameof(BeamOs.Contracts.AnalyticalResults);
    public const string CommonNs = nameof(BeamOs.Contracts.Common);

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
        string[]? additionalNamespaces = null,
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
                c.AdditionalNamespaceUsages = additionalNamespaces ?? [];
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

    public static async Task GenerateOpenApiClient(
        this WebApplication app,
        string releaseName,
        string clientNamespace,
        string clientName,
        string[]? additionalNamespaces = null,
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
                c.AdditionalNamespaceUsages = additionalNamespaces ?? [];
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

    public static async Task GenerateClientsAsync(
        this WebApplication app,
        string openApiDocumentUrl,
        string destinationPath,
        Action<CSharpClientGeneratorSettings>? csSettings,
        Action<TypeScriptClientGeneratorSettings>? tsSettings
    )
    {
        if (app.Configuration["generateclients"] == "true")
        {
            if (tsSettings is null && csSettings is null)
                throw new InvalidOperationException(
                    "Either csharp or typescript client generator settings must be provided!"
                );

            await app.StartAsync();

            var logger = app.Services.GetRequiredService<ILogger<Runner>>();
            logger.LogInformation("Api client generation starting...");

            //var docs = app.Services.GetRequiredService<IOpenApiDocumentGenerator>();
            //var doc = await docs.GenerateAsync(documentName);
            var doc = await OpenApiDocument.FromUrlAsync(openApiDocumentUrl);

            if (csSettings is not null)
            {
                var csGenSettings = new CSharpClientGeneratorSettings
                {
                    ClassName = "ApiClient",
                    CSharpGeneratorSettings = { Namespace = "FastEndpoints" }
                };
                csSettings(csGenSettings);
                var source = new CSharpClientGenerator(doc, csGenSettings).GenerateFile();
                await File.WriteAllTextAsync(
                    Path.Combine(destinationPath, csGenSettings.ClassName + ".cs"),
                    source
                );
                logger.LogInformation("C# api client generation successful!");
            }

            if (tsSettings is not null)
            {
                var tsGenSettings = new TypeScriptClientGeneratorSettings
                {
                    ClassName = "ApiClient",
                    TypeScriptGeneratorSettings = { Namespace = "FastEndpoints" }
                };
                tsSettings(tsGenSettings);
                var source = new TypeScriptClientGenerator(doc, tsGenSettings).GenerateFile();
                await File.WriteAllTextAsync(
                    Path.Combine(destinationPath, tsGenSettings.ClassName + ".ts"),
                    source
                );
                logger.LogInformation("TypeScript api client generation successful!");
            }

            await app.StopAsync();
            Environment.Exit(0);
        }
    }
}
