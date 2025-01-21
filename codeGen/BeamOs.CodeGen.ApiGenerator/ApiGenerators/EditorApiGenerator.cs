using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;

namespace BeamOs.CodeGen.ApiGenerator.ApiGenerators;

public class EditorApiGenerator : AbstractGenerator
{
    public override string ClientName => "EditorApiAlpha";
    protected override string ClientNamespace => "BeamOs.CodeGen.EditorApi";
    protected override string DestinationPath => $"../{this.ClientNamespace}/";
    protected override string OpenApiDefinitionPath => $"/swagger/{this.ClientName}/swagger.json";

    protected override void AddApiMethods(Func<string, RouteHandlerBuilder> addMethod)
    {
        _ = addMethod("CreateElement1d").Accepts<Element1dResponse>();
        _ = addMethod("CreateElement1ds").Accepts<ICollection<Element1dResponse>>();

        _ = addMethod("CreateModel").Accepts<ModelResponse>();

        //_ = addMethod("CreateModelHydrated").Accepts<ModelResponseHydrated>();

        _ = addMethod("CreateNode").Accepts<NodeResponse>();
        _ = addMethod("CreateNodes").Accepts<ICollection<NodeResponse>>();

        _ = addMethod("CreatePointLoad").Accepts<PointLoadResponse>();
        _ = addMethod("CreatePointLoads").Accepts<ICollection<PointLoadResponse>>();

        //_ = addMethod("CreateShearDiagram").Accepts<ShearDiagramResponse>();
        //_ = addMethod("CreateShearDiagrams").Accepts<ShearDiagramResponse[]>();

        //_ = addMethod("CreateMomentDiagram").Accepts<MomentDiagramResponse>();
        //_ = addMethod("CreateMomentDiagrams").Accepts<MomentDiagramResponse[]>();

        _ = addMethod("SetSettings").Accepts<PhysicalModelSettings>();

        //_ = addMethod("SetModelResults").Accepts<AnalyticalResultsResponse>();

        _ = addMethod("Clear");

        _ = addMethod("ClearCurrentOverlay");

        //_ = addMethod("SetColorFilter").Accepts<SetColorFilter>();
        //_ = addMethod("ClearFilters").Accepts<ClearFilters>();

        //foreach (
        //    Type contractType in typeof(IAssemblyMarkerClientActions)
        //        .Assembly
        //        .ExportedTypes
        //        .Where(
        //            t => !t.IsInterface && !t.IsAbstract && t.IsAssignableTo(typeof(IEditorCommand))
        //        )
        //)
        //{
        //    _ = addMethod($"Reduce{contractType.Name}").Accepts(contractType);
        //}
    }

    protected override RouteHandlerBuilder ConfigEachMethod(
        RouteHandlerBuilder routeGroupBuilder
    ) => base.ConfigEachMethod(routeGroupBuilder).Produces<Result>();
}
