using System.Collections.Immutable;
using System.Text.Json.Serialization;
using BeamOs.Common.Contracts;
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

namespace BeamOs.WebApp.Components.Features.Common;

public record CachedModelResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public ModelSettings Settings { get; init; }
    public ImmutableDictionary<int, NodeResponse> Nodes { get; init; }
    public ImmutableDictionary<int, Element1dResponse> Element1ds { get; init; }
    public ImmutableDictionary<int, MaterialResponse> Materials { get; init; }
    public ImmutableDictionary<int, SectionProfileResponse> SectionProfiles { get; init; }
    public ImmutableDictionary<int, PointLoadResponse> PointLoads { get; init; }
    public ImmutableDictionary<int, MomentLoadResponse> MomentLoads { get; init; }

    public ImmutableDictionary<
        int,
        ImmutableDictionary<int, NodeResultResponse>
    >? NodeResults { get; init; }

    public ImmutableDictionary<int, ShearDiagramResponse>? ShearDiagrams { get; init; }
    public ImmutableDictionary<int, MomentDiagramResponse>? MomentDiagrams { get; init; }
    public ImmutableDictionary<int, DeflectionDiagramResponse>? DeflectionDiagrams { get; init; }
    public ImmutableDictionary<int, LoadCase> LoadCases { get; init; }
    public ImmutableDictionary<int, LoadCombination> LoadCombinations { get; init; }

    public CachedModelResponse(ModelResponse modelResponse)
        : this(
            modelResponse.Id,
            modelResponse.Name,
            modelResponse.Description,
            modelResponse.Settings,
            modelResponse.Nodes.ToImmutableDictionary(el => el.Id, el => el),
            modelResponse.Element1ds.ToImmutableDictionary(el => el.Id, el => el),
            modelResponse.Materials.ToImmutableDictionary(el => el.Id, el => el),
            modelResponse.SectionProfiles.ToImmutableDictionary(el => el.Id, el => el),
            modelResponse.PointLoads.ToImmutableDictionary(el => el.Id, el => el),
            modelResponse.MomentLoads.ToImmutableDictionary(el => el.Id, el => el),
            modelResponse.ResultSets.ToImmutableDictionary(
                el => el.Id,
                el => el.NodeResults.ToImmutableDictionary(el => el.NodeId)
            ),
            modelResponse.LoadCases.ToImmutableDictionary(el => el.Id, el => el),
            modelResponse.LoadCombinations.ToImmutableDictionary(el => el.Id, el => el)
        ) { }

    // public CachedModelResponse(ModelResponseHydrated modelResponse)
    //     : this(
    //         modelResponse.Id,
    //         modelResponse.Name,
    //         modelResponse.Description,
    //         modelResponse.Settings,
    //         modelResponse.Nodes.ToImmutableDictionary(el => el.Id, el => el),
    //         modelResponse.Element1ds.ToImmutableDictionary(el => el.Id, el => el),
    //         modelResponse.Materials.ToImmutableDictionary(el => el.Id, el => el),
    //         modelResponse.SectionProfiles.ToImmutableDictionary(el => el.Id, el => el),
    //         modelResponse.PointLoads.ToImmutableDictionary(el => el.Id, el => el),
    //         modelResponse.MomentLoads.ToImmutableDictionary(el => el.Id, el => el),
    //         modelResponse.ResultSets.ToImmutableDictionary(
    //             el => el.Id,
    //             el => el.NodeResults.ToImmutableDictionary(el => el.NodeId)
    //         ),
    //         modelResponse.LoadCases.ToImmutableDictionary(el => el.Id, el => el),
    //         modelResponse.LoadCombinations.ToImmutableDictionary(el => el.Id, el => el)
    //     ) { }

    [JsonConstructor]
    public CachedModelResponse(
        Guid id,
        string name,
        string description,
        ModelSettings settings,
        ImmutableDictionary<int, NodeResponse> nodes,
        ImmutableDictionary<int, Element1dResponse> element1ds,
        ImmutableDictionary<int, MaterialResponse> materials,
        ImmutableDictionary<int, SectionProfileResponse> sectionProfiles,
        ImmutableDictionary<int, PointLoadResponse> pointLoads,
        ImmutableDictionary<int, MomentLoadResponse> momentLoads,
        ImmutableDictionary<int, ImmutableDictionary<int, NodeResultResponse>> nodeResults,
        ImmutableDictionary<int, LoadCase> loadCases,
        ImmutableDictionary<int, LoadCombination> loadCombinations
    )
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Settings = settings;
        this.Nodes = nodes;
        this.Element1ds = element1ds;
        this.Materials = materials;
        this.SectionProfiles = sectionProfiles;
        this.PointLoads = pointLoads;
        this.MomentLoads = momentLoads;
        this.NodeResults = nodeResults;
        this.LoadCases = loadCases;
        this.LoadCombinations = loadCombinations;
    }
}

[JsonSerializable(typeof(Result<CachedModelResponse>))]
[JsonSerializable(typeof(CachedModelResponse))]
public partial class BeamOsWebAppJsonSerializerContext : JsonSerializerContext { }
