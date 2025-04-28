using System.Text.Json;
using System.Text.Json.Serialization;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.NodeResult;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

[JsonSerializable(typeof(CreateNodeRequest))]
[JsonSerializable(typeof(CreateModelRequest))]
[JsonSerializable(typeof(CreateMaterialRequest))]
[JsonSerializable(typeof(CreateElement1dRequest))]
[JsonSerializable(typeof(CreateSectionProfileRequest))]
[JsonSerializable(typeof(CreatePointLoadRequest))]
[JsonSerializable(typeof(CreateMomentLoadRequest))]
[JsonSerializable(typeof(PutNodeRequest))]
[JsonSerializable(typeof(NodeData))]
[JsonSerializable(typeof(IEnumerable<PutNodeRequest>))]
[JsonSerializable(typeof(PutMaterialRequest))]
[JsonSerializable(typeof(IEnumerable<PutMaterialRequest>))]
[JsonSerializable(typeof(MaterialRequestData))]
[JsonSerializable(typeof(PutElement1dRequest))]
[JsonSerializable(typeof(IEnumerable<PutElement1dRequest>))]
[JsonSerializable(typeof(Element1dData))]
[JsonSerializable(typeof(PutSectionProfileRequest))]
[JsonSerializable(typeof(SectionProfileData))]
[JsonSerializable(typeof(IEnumerable<PutSectionProfileRequest>))]
[JsonSerializable(typeof(PutPointLoadRequest))]
[JsonSerializable(typeof(PointLoadData))]
[JsonSerializable(typeof(IEnumerable<PutPointLoadRequest>))]
[JsonSerializable(typeof(PutMomentLoadRequest))]
[JsonSerializable(typeof(IEnumerable<PutMomentLoadRequest>))]
[JsonSerializable(typeof(UpdateNodeRequest))]
[JsonSerializable(typeof(NodeResponse))]
[JsonSerializable(typeof(SpeckleReceiveParameters))]
[JsonSerializable(typeof(Result<NodeResponse>))]
[JsonSerializable(typeof(Result<ModelResponse>))]
[JsonSerializable(typeof(Result<ModelResponseHydrated>))]
[JsonSerializable(typeof(Result<Element1dResponse>))]
[JsonSerializable(typeof(Result<MaterialResponse>))]
[JsonSerializable(typeof(Result<SectionProfileResponse>))]
[JsonSerializable(typeof(Result<PointLoadResponse>))]
[JsonSerializable(typeof(Result<MomentLoadResponse>))]
[JsonSerializable(typeof(Result<NodeResultResponse>))]
[JsonSerializable(typeof(Result<ResultSetResponse>))]
[JsonSerializable(typeof(Result<ModelEntityResponse>))]
[JsonSerializable(typeof(Result<List<ModelInfoResponse>>))]
[JsonSerializable(typeof(Result<AnalyticalResultsResponse>))]
[JsonSerializable(typeof(Result<BatchResponse>))]
[JsonSerializable(typeof(Result<BeamOsModelBuilderDto>))]
[JsonSerializable(typeof(DiagramConsistentIntervalResponse))]
[JsonSerializable(typeof(MomentDiagramResponse))]
[JsonSerializable(typeof(Result<int>))]
[JsonSerializable(typeof(Result<bool>))]
[JsonSerializable(typeof(Result<string>))]
[JsonSerializable(typeof(Result))]
[JsonSerializable(typeof(GithubModelsChatRequest))]
internal partial class BeamOsJsonSerializerContext : JsonSerializerContext { }

public static class BeamOsSerializerOptions
{
    private static JsonSerializerOptions? options;
    public static JsonSerializerOptions Default
    {
        get
        {
            if (options is null)
            {
                options = new() { PropertyNameCaseInsensitive = true };
                options.TypeInfoResolverChain.Insert(0, BeamOsJsonSerializerContext.Default);
                options.Converters.Add(new JsonStringEnumConverter());
            }
            return options;
        }
    }

    public static Action<JsonSerializerOptions> DefaultConfig { get; } =
        static (options) =>
        {
            options.PropertyNameCaseInsensitive = true;
            options.TypeInfoResolverChain.Insert(0, BeamOsJsonSerializerContext.Default);
            options.Converters.Add(new JsonStringEnumConverter());
        };
}
