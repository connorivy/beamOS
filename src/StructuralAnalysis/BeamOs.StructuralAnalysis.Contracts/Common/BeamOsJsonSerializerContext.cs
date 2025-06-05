using System.Text.Json;
using System.Text.Json.Serialization;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.NodeResult;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using LoadCase = BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases.LoadCase;

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
[JsonSerializable(typeof(CreateInternalNodeRequest))]
[JsonSerializable(typeof(InternalNodeData))]
[JsonSerializable(typeof(IEnumerable<InternalNodeData>))]
[JsonSerializable(typeof(Result<InternalNode>))]
[JsonSerializable(typeof(PutMaterialRequest))]
[JsonSerializable(typeof(IEnumerable<PutMaterialRequest>))]
[JsonSerializable(typeof(MaterialRequestData))]
[JsonSerializable(typeof(PutElement1dRequest))]
[JsonSerializable(typeof(IEnumerable<PutElement1dRequest>))]
[JsonSerializable(typeof(Element1dData))]
[JsonSerializable(typeof(PutSectionProfileRequest))]
[JsonSerializable(typeof(SectionProfileData))]
[JsonSerializable(typeof(SectionProfileFromLibraryData))]
[JsonSerializable(typeof(IEnumerable<PutSectionProfileRequest>))]
[JsonSerializable(typeof(PutPointLoadRequest))]
[JsonSerializable(typeof(PointLoadData))]
[JsonSerializable(typeof(IEnumerable<PutPointLoadRequest>))]
[JsonSerializable(typeof(PutMomentLoadRequest))]
[JsonSerializable(typeof(IEnumerable<PutMomentLoadRequest>))]
[JsonSerializable(typeof(UpdateNodeRequest))]
[JsonSerializable(typeof(NodeResponse))]
[JsonSerializable(typeof(SpeckleReceiveParameters))]
[JsonSerializable(typeof(RunDsmRequest))]
[JsonSerializable(typeof(LoadCaseData))]
[JsonSerializable(typeof(IEnumerable<LoadCase>))]
[JsonSerializable(typeof(Dictionary<int, double>))]
[JsonSerializable(typeof(LoadCombinationData))]
[JsonSerializable(typeof(ModelProposalData))]
[JsonSerializable(typeof(Result<ModelProposalResponse>))]
[JsonSerializable(typeof(Result<List<ModelProposalInfo>>))]
[JsonSerializable(typeof(IEnumerable<LoadCombination>))]
[JsonSerializable(typeof(Result<NodeResponse>))]
[JsonSerializable(typeof(Result<ModelResponse>))]
[JsonSerializable(typeof(Result<ModelResponseHydrated>))]
[JsonSerializable(typeof(Result<Element1dResponse>))]
[JsonSerializable(typeof(Result<MaterialResponse>))]
[JsonSerializable(typeof(Result<SectionProfileResponse>))]
[JsonSerializable(typeof(Result<SectionProfileFromLibrary>))]
[JsonSerializable(typeof(Result<PointLoadResponse>))]
[JsonSerializable(typeof(Result<MomentLoadResponse>))]
[JsonSerializable(typeof(Result<NodeResultResponse>))]
[JsonSerializable(typeof(Result<ResultSetResponse>))]
[JsonSerializable(typeof(Result<ModelEntityResponse>))]
[JsonSerializable(typeof(Result<List<ModelInfoResponse>>))]
[JsonSerializable(typeof(Result<AnalyticalResultsResponse>))]
[JsonSerializable(typeof(Result<BatchResponse>))]
[JsonSerializable(typeof(Result<BeamOsModelBuilderDto>))]
[JsonSerializable(typeof(Result<LoadCase>))]
[JsonSerializable(typeof(Result<LoadCombination>))]
[JsonSerializable(typeof(DiagramConsistentIntervalResponse))]
[JsonSerializable(typeof(MomentDiagramResponse))]
[JsonSerializable(typeof(Result<int>))]
[JsonSerializable(typeof(Result<bool>))]
[JsonSerializable(typeof(Result<string>))]
[JsonSerializable(typeof(Result))]
[JsonSerializable(typeof(GithubModelsChatRequest))]
[JsonSerializable(typeof(Result<GithubModelsChatResponse>))]
internal partial class BeamOsJsonSerializerContext : JsonSerializerContext { }

public static class BeamOsSerializerOptions
{
    private static readonly Lock OptionsLock = new();
    private static readonly Lock PrettyOptionsLock = new();

    public static JsonSerializerOptions Default
    {
        get
        {
            lock (OptionsLock)
            {
                if (field is null)
                {
                    field = new() { PropertyNameCaseInsensitive = true };
                    field.TypeInfoResolverChain.Insert(0, BeamOsJsonSerializerContext.Default);
                    field.Converters.Add(new JsonStringEnumConverter());
                }
            }
            return field;
        }
    }
    public static JsonSerializerOptions Pretty
    {
        get
        {
            lock (PrettyOptionsLock)
            {
                if (field is null)
                {
                    field = new() { PropertyNameCaseInsensitive = true, WriteIndented = true };
                    field.TypeInfoResolverChain.Insert(0, BeamOsJsonSerializerContext.Default);
                    field.Converters.Add(new JsonStringEnumConverter());
                }
            }
            return field;
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
