using BeamOs.CodeGen.Apis.Generator.Apis;
using FastEndpoints.ClientGen;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.TypeScript;

namespace BeamOs.CodeGen.Apis.Generator.Apis;

public abstract class AbstractGenerator
{
    protected WebApplication App { get; }
    private readonly RouteGroupBuilder routeGroupBuilder;

    public AbstractGenerator(WebApplication app)
    {
        this.App = app;
        this.routeGroupBuilder = app.MapGroup(this.ClientName)
            .WithGroupName(this.ClientName)
            .WithOpenApi();
    }

    protected abstract string DestinationPath { get; }
    protected abstract string ClientName { get; }
    protected virtual string ClientNamespace { get; } = "";
    protected abstract string OpenApiDefinitionUrl { get; }
    protected virtual bool GenerateCsClient { get; } = true;
    protected virtual bool GenerateTsClient { get; } = true;

    protected RouteHandlerBuilder AddMethodToApi(string methodName)
    {
        return this.routeGroupBuilder.MapPost(methodName, () => TypedResults.Ok());
    }

    public async Task GenerateClients()
    {
        var httpClient = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage
        {
            RequestUri = new Uri(this.OpenApiDefinitionUrl),
            Method = HttpMethod.Get
        };

        try
        {
            var result = await httpClient.SendAsync(request);
        }
        catch (System.Net.Http.HttpRequestException)
        {
            await this.App.StartAsync();
        }

        var logger = this.App.Services.GetRequiredService<ILogger<Runner>>();
        logger.LogInformation("Api client generation starting...");

        var doc = await OpenApiDocument.FromUrlAsync(this.OpenApiDefinitionUrl);

        if (this.GenerateCsClient)
        {
            var csGenSettings = new CSharpClientGeneratorSettings
            {
                ClassName = this.ClientName,
                CSharpGeneratorSettings = { Namespace = this.ClientNamespace },
                GenerateClientInterfaces = true,
                GenerateDtoTypes = false,
                UseBaseUrl = false
            };

            var source = new CSharpClientGenerator(doc, csGenSettings).GenerateFile();
            await File.WriteAllTextAsync(
                Path.Combine(this.DestinationPath, csGenSettings.ClassName + ".cs"),
                source
            );
            logger.LogInformation("C# api client generation successful!");
        }

        if (this.GenerateTsClient)
        {
            var tsGenSettings = new TypeScriptClientGeneratorSettings
            {
                ClassName = this.ClientName,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings = { Namespace = "" } // needed to not generate a namespace
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
            logger.LogInformation("TypeScript api client generation successful!");
        }

        //await this.App.StopAsync();
        //Environment.Exit(0);
    }
}

public static class ApiGeneratorUtils
{
    public static RouteHandlerBuilder Accepts<T>(this RouteHandlerBuilder routeHandlerBuilder)
    {
        return routeHandlerBuilder.Accepts<T>("application/json");
    }
}
