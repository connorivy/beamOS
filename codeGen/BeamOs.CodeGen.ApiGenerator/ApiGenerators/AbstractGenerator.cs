using BeamOs.CodeGen.ApiGenerator.ApiGenerators;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.TypeScript;

namespace BeamOs.CodeGen.ApiGenerator.ApiGenerators;

public abstract class AbstractGenerator : IApiGenerator
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

    public async Task GenerateClients()
    {
        Console.WriteLine("Api client generation starting...");

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
                CSharpGeneratorSettings =
                {
                    Namespace = this.ClientNamespace,
                    JsonLibrary = CSharpJsonLibrary.SystemTextJson,
                },
                GenerateClientInterfaces = true,
                GenerateDtoTypes = false,
                UseBaseUrl = false,
                ExceptionClass = $"{this.ClientName}Exception"
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
            Console.WriteLine("TypeScript api client generation successful!");
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
