using BeamOs.Common.Api;
using BeamOs.Contracts.AnalyticalResults.Diagrams;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.WebApp.Client.EditorCommands;
using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOs.CodeGen.Apis.Generator.ApiGenerators;

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

        _ = addMethod("CreatePointLoad").Accepts<PointLoadResponse>();

        _ = addMethod("CreateShearDiagram").Accepts<ShearDiagramResponse>();

        _ = addMethod("Clear");

        foreach (
            Type contractType in typeof(IAssemblyMarkerClientActions)
                .Assembly
                .ExportedTypes
                .Where(
                    t => !t.IsInterface && !t.IsAbstract && t.IsAssignableTo(typeof(IEditorCommand))
                )
        )
        {
            _ = addMethod($"Reduce{contractType.Name}").Accepts(contractType);
        }
    }

    protected override RouteHandlerBuilder ConfigEachMethod(
        RouteHandlerBuilder routeGroupBuilder
    ) => base.ConfigEachMethod(routeGroupBuilder).Produces<Result>();
}
