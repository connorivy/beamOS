//using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.TypeScript;

namespace BeamOs.CodeGen.ApiGenerator.ApiGenerators;

public interface IApiGenerator
{
    public Task GenerateClients();
}

public abstract class AbstractGeneratorFromApiProject<TAssemblyMarker> : IApiGenerator
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
        using var appFactory = new WebApplicationFactory<TAssemblyMarker>().WithWebHostBuilder(
            //builder => builder.UseSolutionRelativeContentRoot(Environment.CurrentDirectory)
            builder =>
            {
                builder.UseSolutionRelativeContentRoot(Environment.CurrentDirectory);
                builder.UseDefaultServiceProvider(options => options.ValidateScopes = false);
            }
        );
        HttpClient client = appFactory.CreateClient();

        var json = await client.GetStringAsync(this.OpenApiDefinitionPath);

        var doc = await OpenApiDocument.FromJsonAsync(json);

        if (this.GenerateCsClient)
        {
            var csGenSettings = new CSharpClientGeneratorSettings
            {
                ClassName = this.ClientName,
                CSharpGeneratorSettings =
                {
                    Namespace = this.ClientNamespace,
                    JsonLibrary = CSharpJsonLibrary.SystemTextJson,
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
                TypeScriptGeneratorSettings =
                {
                    Namespace = "",
                } // needed to not generate a namespace
                ,
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
    }
}
