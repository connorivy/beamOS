//using Microsoft.OpenApi.Models;
using BeamOs.StructuralAnalysis.Api;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.TypeScript;
using Scalar.AspNetCore;

namespace BeamOs.CodeGen.ApiGenerator.ApiGenerators;

public abstract class AbstractGeneratorFromEndpoints<TAssemblyMarker> : IApiGenerator
    where TAssemblyMarker : class
{
    protected abstract string DestinationPath { get; }
    public abstract string ClientName { get; }
    protected virtual string ClientNamespace { get; } = "";
    protected abstract string OpenApiDefinitionPath { get; }
    protected virtual bool GenerateCsClient { get; } = true;
    protected virtual bool GenerateTsClient { get; } = true;

    public async Task GenerateClients()
    {
        var builder = WebApplication.CreateBuilder();

#if DEBUG
        builder.Services.AddOpenApi();
#endif

        WebApplication app = builder.Build();

        app.MapEndpoints<TAssemblyMarker>();

#if DEBUG
        app.MapOpenApi();
        app.MapScalarApiReference();
#endif

        var baseUrl = app.Configuration[$"URLS"].Split(';').First();
        await app.StartAsync();

        UriBuilder uriBuilder = new(baseUrl) { Path = this.OpenApiDefinitionPath };

        var doc = await OpenApiDocument.FromUrlAsync(uriBuilder.ToString());

        if (this.GenerateCsClient)
        {
            var csGenSettings = new CSharpClientGeneratorSettings
            {
                ClassName = this.ClientName,
                CSharpGeneratorSettings =
                {
                    Namespace = this.ClientNamespace,
                    JsonLibrary = CSharpJsonLibrary.SystemTextJson,
                    TemplateDirectory = "./Templates",
                },
                GenerateClientInterfaces = true,
                GenerateDtoTypes = false,
                UseBaseUrl = false,
                ExceptionClass = $"{this.ClientName}Exception",
                GenerateOptionalParameters = true,
            };

            var source = new CSharpClientGenerator(doc, csGenSettings).GenerateFile();

            if (!Path.Exists(this.DestinationPath))
            {
                Directory.CreateDirectory(this.DestinationPath);
            }

            await File.WriteAllTextAsync(
                Path.Combine(this.DestinationPath, csGenSettings.ClassName + ".cs"),
                source
            );
            Console.WriteLine("C# api client generation successful!");
        }

        if (this.GenerateTsClient)
        {
            var tsGenSettings = new TypeScriptClientGeneratorSettings
            {
                ClassName = this.ClientName,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings = { Namespace = "" }, // needed to not generate a namespace
            };

            var source = new TypeScriptClientGenerator(doc, tsGenSettings).GenerateFile();

            if (!Path.Exists(this.DestinationPath))
            {
                Directory.CreateDirectory(this.DestinationPath);
            }

            await File.WriteAllTextAsync(
                Path.Combine(this.DestinationPath, tsGenSettings.ClassName + ".ts"),
                source
            );
            Console.WriteLine("TypeScript api client generation successful!");
        }

        await app.StopAsync();
    }
}
