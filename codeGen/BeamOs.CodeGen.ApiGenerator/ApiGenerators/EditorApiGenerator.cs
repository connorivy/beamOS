using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.WebApp.EditorCommands;
using BeamOs.WebApp.EditorCommands.Interfaces;

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

        _ = addMethod("DeleteElement1d").Accepts<IModelEntity>();
        _ = addMethod("DeleteElement1ds").Accepts<ICollection<IModelEntity>>();

        _ = addMethod("CreateModel").Accepts<ModelResponse>();

        //_ = addMethod("CreateModelHydrated").Accepts<ModelResponseHydrated>();

        _ = addMethod("CreateNode").Accepts<NodeResponse>();
        _ = addMethod("CreateNodes").Accepts<ICollection<NodeResponse>>();

        _ = addMethod("UpdateNode").Accepts<NodeResponse>();
        _ = addMethod("UpdateNodes").Accepts<ICollection<NodeResponse>>();

        _ = addMethod("DeleteNode").Accepts<IModelEntity>();
        _ = addMethod("DeleteNodes").Accepts<ICollection<IModelEntity>>();

        _ = addMethod("CreatePointLoad").Accepts<PointLoadResponse>();
        _ = addMethod("CreatePointLoads").Accepts<ICollection<PointLoadResponse>>();

        _ = addMethod("DeletePointLoad").Accepts<IModelEntity>();
        _ = addMethod("DeletePointLoads").Accepts<ICollection<IModelEntity>>();

        _ = addMethod("CreateShearDiagram").Accepts<ShearDiagramResponse>();
        _ = addMethod("CreateShearDiagrams").Accepts<ShearDiagramResponse[]>();

        _ = addMethod("CreateMomentDiagram").Accepts<MomentDiagramResponse>();
        _ = addMethod("CreateMomentDiagrams").Accepts<MomentDiagramResponse[]>();

        _ = addMethod("CreateDeflectionDiagram").Accepts<DeflectionDiagramResponse>();
        _ = addMethod("CreateDeflectionDiagrams").Accepts<DeflectionDiagramResponse[]>();

        _ = addMethod("SetSettings").Accepts<ModelSettings>();

        _ = addMethod("SetGlobalStresses").Accepts<GlobalStresses>();

        _ = addMethod("Clear");

        _ = addMethod("ClearCurrentOverlay");

        //_ = addMethod("SetColorFilter").Accepts<SetColorFilter>();
        //_ = addMethod("ClearFilters").Accepts<ClearFilters>();

        foreach (
            Type contractType in typeof(IAssemblyMarkerClientCommands)
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
