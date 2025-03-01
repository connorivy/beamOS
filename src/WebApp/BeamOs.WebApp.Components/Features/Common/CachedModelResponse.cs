using System.Collections.Immutable;
using System.Text.Json.Serialization;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.NodeResult;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.WebApp.Components.Features.Common;

public record CachedModelResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public PhysicalModelSettings Settings { get; init; }
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
            modelResponse
                .ResultSets
                .ToImmutableDictionary(
                    el => el.Id,
                    el => el.NodeResults.ToImmutableDictionary(el => el.NodeId)
                )
        ) { }

    public CachedModelResponse(ModelResponseHydrated modelResponse)
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
            modelResponse
                .ResultSets
                .ToImmutableDictionary(
                    el => el.Id,
                    el => el.NodeResults.ToImmutableDictionary(el => el.NodeId)
                )
        ) { }

    [JsonConstructor]
    public CachedModelResponse(
        Guid id,
        string name,
        string description,
        PhysicalModelSettings settings,
        ImmutableDictionary<int, NodeResponse> nodes,
        ImmutableDictionary<int, Element1dResponse> element1ds,
        ImmutableDictionary<int, MaterialResponse> materials,
        ImmutableDictionary<int, SectionProfileResponse> sectionProfiles,
        ImmutableDictionary<int, PointLoadResponse> pointLoads,
        ImmutableDictionary<int, MomentLoadResponse> momentLoads,
        ImmutableDictionary<int, ImmutableDictionary<int, NodeResultResponse>> nodeResults
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
    }
}

[JsonSerializable(typeof(Result<CachedModelResponse>))]
[JsonSerializable(typeof(CachedModelResponse))]
public partial class BeamOsWebAppJsonSerializerContext : JsonSerializerContext { }
