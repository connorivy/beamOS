using BeamOs.Common.Api;
using BeamOs.Contracts.AnalyticalModel.Diagrams;
using BeamOs.Contracts.AnalyticalModel.Results;
using BeamOs.Contracts.Editor;
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
        _ = addMethod("CreateElement1ds").Accepts<ICollection<Element1DResponse>>();

        _ = addMethod("CreateModel").Accepts<ModelResponse>();

        _ = addMethod("CreateModelHydrated").Accepts<ModelResponseHydrated>();

        _ = addMethod("CreateNode").Accepts<NodeResponse>();
        _ = addMethod("CreateNodes").Accepts<ICollection<NodeResponse>>();

        _ = addMethod("CreatePointLoad").Accepts<PointLoadResponse>();
        _ = addMethod("CreatePointLoads").Accepts<ICollection<PointLoadResponse>>();

        _ = addMethod("CreateShearDiagram").Accepts<ShearDiagramResponse>();
        _ = addMethod("CreateShearDiagrams").Accepts<ShearDiagramResponse[]>();

        _ = addMethod("CreateMomentDiagram").Accepts<MomentDiagramResponse>();
        _ = addMethod("CreateMomentDiagrams").Accepts<MomentDiagramResponse[]>();

        _ = addMethod("SetSettings").Accepts<PhysicalModelSettings>();

        _ = addMethod("SetModelResults").Accepts<AnalyticalResultsResponse>();

        _ = addMethod("Clear");

        _ = addMethod("ClearCurrentOverlay");

        _ = addMethod("SetColorFilter").Accepts<SetColorFilter>();
        _ = addMethod("ClearFilters").Accepts<ClearFilters>();

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
