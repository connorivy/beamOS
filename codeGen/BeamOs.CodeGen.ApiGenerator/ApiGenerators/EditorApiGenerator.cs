using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.WebApp.EditorCommands;
using BeamOs.WebApp.EditorCommands.Interfaces;

namespace BeamOs.CodeGen.ApiGenerator.ApiGenerators;

public class EditorApiGenerator : AbstractGenerator2
{
    public override string ClientName => "EditorApiAlpha";
    protected override string ClientNamespace => "BeamOs.CodeGen.EditorApi";
    protected override string DestinationPath => $"../{this.ClientNamespace}/";
    protected override string OpenApiDefinitionPath => "openapi/v1.json";

    // no custom templates for this builder
    protected override string? TemplateDirectory => null;


    protected override void MapEndpoints(WebApplication app)
    {
        var builder = app;

        builder.AddResultMethod<Element1dResponse>("CreateElement1d");
        builder.AddResultMethod<ICollection<Element1dResponse>>("CreateElement1ds");

        builder.AddResultMethod<Element1dResponse>("UpdateElement1d");
        builder.AddResultMethod<ICollection<Element1dResponse>>("UpdateElement1ds");

        builder.AddResultMethod<IModelEntity>("DeleteElement1d");
        builder.AddResultMethod<ICollection<IModelEntity>>("DeleteElement1ds");

        builder.AddResultMethod<ModelResponse>("CreateModel");

        builder.AddResultMethod<NodeResponse>("CreateNode");
        builder.AddResultMethod<ICollection<NodeResponse>>("CreateNodes");

        builder.AddResultMethod<NodeResponse>("UpdateNode");
        builder.AddResultMethod<ICollection<NodeResponse>>("UpdateNodes");

        builder.AddResultMethod<IModelEntity>("DeleteNode");
        builder.AddResultMethod<ICollection<IModelEntity>>("DeleteNodes");

        builder.AddResultMethod<PointLoadResponse>("CreatePointLoad");
        builder.AddResultMethod<ICollection<PointLoadResponse>>("CreatePointLoads");

        builder.AddResultMethod<PointLoadResponse>("UpdatePointLoad");
        builder.AddResultMethod<ICollection<PointLoadResponse>>("UpdatePointLoads");

        builder.AddResultMethod<IModelEntity>("DeletePointLoad");
        builder.AddResultMethod<ICollection<IModelEntity>>("DeletePointLoads");

        builder.AddResultMethod<ShearDiagramResponse>("CreateShearDiagram");
        builder.AddResultMethod<ICollection<ShearDiagramResponse>>("CreateShearDiagrams");

        builder.AddResultMethod<MomentDiagramResponse>("CreateMomentDiagram");
        builder.AddResultMethod<ICollection<MomentDiagramResponse>>("CreateMomentDiagrams");

        builder.AddResultMethod<DeflectionDiagramResponse>("CreateDeflectionDiagram");
        builder.AddResultMethod<ICollection<DeflectionDiagramResponse>>("CreateDeflectionDiagrams");

        builder.AddResultMethod<ModelProposalResponse>("DisplayModelProposal");
        builder.AddResultMethod("ClearModelProposals");

        builder.AddResultMethod<ModelSettings>("SetSettings");

        builder.AddResultMethod<GlobalStresses>("SetGlobalStresses");

        builder.AddResultMethod("Clear");

        builder.AddResultMethod("ClearCurrentOverlay");

        foreach (
            Type contractType in typeof(IAssemblyMarkerClientCommands).Assembly.ExportedTypes.Where(
                t => !t.IsInterface && !t.IsAbstract && t.IsAssignableTo(typeof(IEditorCommand))
            )
        )
        {
            builder.AddResultMethod(
                contractType,
                $"Reduce{contractType.Name}"
            );
        }
    }
}

public static class RouteGroupBuilderExtensions
{
    public static RouteHandlerBuilder AddMethod<TAccept, TReturn>(this IEndpointRouteBuilder builder, string methodName)
        where TAccept : notnull
    {
        return builder.MapPost(
            methodName,
            () => Results.Ok()
        ).Accepts<TAccept>()
        .Produces<TReturn>();
    }

    public static RouteHandlerBuilder AddMethod<TReturn>(this IEndpointRouteBuilder builder, string methodName)
    {
        return builder.MapPost(
            methodName,
            () => Results.Ok()
        )
        .Produces<TReturn>();
    }

    public static RouteHandlerBuilder AddResultMethod<TAccept>(
        this IEndpointRouteBuilder builder,
        string methodName
    )
        where TAccept : notnull
    {
        return builder.AddMethod<TAccept, Result>(methodName);
    }
    public static RouteHandlerBuilder AddResultMethod(
        this IEndpointRouteBuilder builder,
        Type acceptType,
        string methodName
    )
    {
        return builder.AddMethod<Result>(methodName).Accepts(acceptType);
    }
    public static RouteHandlerBuilder AddResultMethod(
        this IEndpointRouteBuilder builder,
        string methodName
    )
    {
        return builder.AddMethod<Result>(methodName);
    }
}
