using System.Text.Json.Serialization;
using BeamOs.Common.Contracts;
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
    public Dictionary<int, NodeResponse> Nodes { get; init; }
    public Dictionary<int, Element1dResponse> Element1ds { get; init; }
    public Dictionary<int, MaterialResponse> Materials { get; init; }
    public Dictionary<int, SectionProfileResponse> SectionProfiles { get; init; }
    public Dictionary<int, PointLoadResponse> PointLoads { get; init; }
    public Dictionary<int, MomentLoadResponse> MomentLoads { get; init; }

    public CachedModelResponse(ModelResponse modelResponse)
        : this(
            modelResponse.Id,
            modelResponse.Name,
            modelResponse.Description,
            modelResponse.Settings,
            modelResponse.Nodes.ToDictionary(el => el.Id, el => el),
            modelResponse.Element1ds.ToDictionary(el => el.Id, el => el),
            modelResponse.Materials.ToDictionary(el => el.Id, el => el),
            modelResponse.SectionProfiles.ToDictionary(el => el.Id, el => el),
            modelResponse.PointLoads.ToDictionary(el => el.Id, el => el),
            modelResponse.MomentLoads.ToDictionary(el => el.Id, el => el)
        ) { }

    public CachedModelResponse(ModelResponseHydrated modelResponse)
        : this(
            modelResponse.Id,
            modelResponse.Name,
            modelResponse.Description,
            modelResponse.Settings,
            modelResponse.Nodes.ToDictionary(el => el.Id, el => el),
            modelResponse.Element1ds.ToDictionary(el => el.Id, el => el),
            modelResponse.Materials.ToDictionary(el => el.Id, el => el),
            modelResponse.SectionProfiles.ToDictionary(el => el.Id, el => el),
            modelResponse.PointLoads.ToDictionary(el => el.Id, el => el),
            modelResponse.MomentLoads.ToDictionary(el => el.Id, el => el)
        ) { }

    [JsonConstructor]
    public CachedModelResponse(
        Guid id,
        string name,
        string description,
        PhysicalModelSettings settings,
        Dictionary<int, NodeResponse> nodes,
        Dictionary<int, Element1dResponse> element1ds,
        Dictionary<int, MaterialResponse> materials,
        Dictionary<int, SectionProfileResponse> sectionProfiles,
        Dictionary<int, PointLoadResponse> pointLoads,
        Dictionary<int, MomentLoadResponse> momentLoads
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
    }
}

[JsonSerializable(typeof(Result<CachedModelResponse>))]
[JsonSerializable(typeof(CachedModelResponse))]
public partial class BeamOsWebAppJsonSerializerContext : JsonSerializerContext { }
