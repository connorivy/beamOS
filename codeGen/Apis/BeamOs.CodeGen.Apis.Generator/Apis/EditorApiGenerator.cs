using BeamOs.Common.Api;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;

namespace BeamOs.CodeGen.Apis.Generator.Apis;

public class EditorApiGenerator : AbstractGenerator
{
    public override string ClientName => "EditorApiAlpha";
    protected override string ClientNamespace => "BeamOs.CodeGen.Apis.EditorApi";
    protected override string DestinationPath => $"../{this.ClientNamespace}/";
    protected override string OpenApiDefinitionPath => $"/swagger/{this.ClientName}/swagger.json";

    protected override void AddApiMethods(Func<string, RouteHandlerBuilder> addMethod)
    {
        _ = addMethod("CreateElement1d").Accepts<Element1DResponse>();

        _ = addMethod("CreateModel").Accepts<ModelResponse>();

        _ = addMethod("CreateModelHydrated").Accepts<ModelResponseHydrated>();

        _ = addMethod("CreateNode").Accepts<NodeResponse>();

        _ = addMethod("Clear");
    }

    protected override RouteHandlerBuilder ConfigEachMethod(
        RouteHandlerBuilder routeGroupBuilder
    ) => base.ConfigEachMethod(routeGroupBuilder).Produces<Result>();
}
