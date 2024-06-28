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
        _ = this.AddMethodToApi("CreateElement1d").Accepts<Element1DResponse>();

        _ = this.AddMethodToApi("CreateModel").Accepts<ModelResponse>();

        _ = this.AddMethodToApi("CreateModelHydrated").Accepts<ModelResponseHydrated>();

        _ = this.AddMethodToApi("CreateNode").Accepts<NodeResponse>();

        _ = this.AddMethodToApi("Clear");
    }

    public const string EditorApiDocumentName = "EditorApiAlpha";
    protected override string ClientName => EditorApiDocumentName;
    protected override string ClientNamespace => "BeamOs.CodeGen.Apis.EditorApi";
    protected override string DestinationPath => $"../{this.ClientNamespace}/";
    protected override string OpenApiDefinitionUrl =>
        $"{this.App.Configuration[$"URLS"].Split(';').First()}/swagger/{this.ClientName}/swagger.json";

    protected override RouteHandlerBuilder AddMethodToApi(string methodName) =>
        base.AddMethodToApi(methodName).Produces<Result>();
}
