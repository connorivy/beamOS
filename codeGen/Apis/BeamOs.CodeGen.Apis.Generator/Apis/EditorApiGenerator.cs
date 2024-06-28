using BeamOs.Common.Api;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;

namespace BeamOs.CodeGen.Apis.Generator.Apis;

public class EditorApiGenerator : AbstractGenerator
{
    public EditorApiGenerator(WebApplication app)
        : base(app)
    {
        _ = this.AddMethodToApi("CreateElement1d").Accepts<Element1DResponse>().Produces<Result>();

        _ = this.AddMethodToApi("CreateModel").Accepts<ModelResponse>().Produces<Result>();

        _ = this.AddMethodToApi("CreateModelHydrated")
            .Accepts<ModelResponseHydrated>()
            .Produces<Result>();

        _ = this.AddMethodToApi("CreateNode").Accepts<NodeResponse>().Produces<Result>();

        _ = this.AddMethodToApi("Clear").Produces<Result>();
    }

    public const string EditorApiDocumentName = "EditorApiAlpha";
    protected override string ClientName => EditorApiDocumentName;
    protected override string ClientNamespace => "BeamOS.CodeGen.Apis.EditorApi";
    protected override string DestinationPath => $"../{this.ClientNamespace}/";
    protected override string OpenApiDefinitionUrl =>
        $"{this.App.Configuration[$"URLS"].Split(';').First()}/swagger/{this.ClientName}/swagger.json";
}

public static class EditorApiGeneratorUtils
{
    public static async Task GenerateEditorApi(this WebApplication app)
    {
        EditorApiGenerator editorApiGenerator = new(app);
        await editorApiGenerator.GenerateClients();
    }
}
