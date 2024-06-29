using BeamOs.CodeGen.Apis.Generator.Apis;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.TypeScript;

namespace BeamOs.CodeGen.Apis.Generator.Apis;

public abstract class AbstractGenerator
{
    protected abstract string DestinationPath { get; }
    public abstract string ClientName { get; }
    protected virtual string ClientNamespace { get; } = "";
    protected abstract string OpenApiDefinitionPath { get; }
    protected virtual bool GenerateCsClient { get; } = true;
    protected virtual bool GenerateTsClient { get; } = true;

    private string? baseUrl;

    public void AddMethods(WebApplication app)
    {
        this.baseUrl = app.Configuration[$"URLS"].Split(';').First();

        RouteGroupBuilder builder = app.MapGroup(this.ClientName)
            .WithGroupName(this.ClientName)
            .WithOpenApi();

        this.AddApiMethods(this.AddSingleMethodToApi(builder));
    }

    private Func<string, RouteHandlerBuilder> AddSingleMethodToApi(
        RouteGroupBuilder routeGroupBuilder
    )
    {
        return (methodName) =>
            this.ConfigEachMethod(routeGroupBuilder.MapPost(methodName, () => TypedResults.Ok()));
    }

    protected virtual RouteHandlerBuilder ConfigEachMethod(RouteHandlerBuilder routeGroupBuilder) =>
        routeGroupBuilder;

    protected abstract void AddApiMethods(Func<string, RouteHandlerBuilder> addMethod);

    public async Task GenerateClients(ILogger logger)
    {
        logger.LogInformation("Api client generation starting...");

        UriBuilder uriBuilder =
            new(
                this.baseUrl
                    ?? throw new Exception(
                        $"baseUrl was null. Need to call the '{nameof(AddMethods)}' method before client can be generated"
                    )
            )
            {
                Path = this.OpenApiDefinitionPath
            };

        var doc = await OpenApiDocument.FromUrlAsync(uriBuilder.ToString());

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
    }
}

public static class ApiGeneratorUtils
{
    public static RouteHandlerBuilder Accepts<T>(this RouteHandlerBuilder routeHandlerBuilder)
        where T : notnull
    {
        return routeHandlerBuilder.Accepts<T>("application/json");
    }

    public static RouteHandlerBuilder Accepts(this RouteHandlerBuilder routeHandlerBuilder, Type t)
    {
        return routeHandlerBuilder.Accepts(t, "application/json");
    }
}
