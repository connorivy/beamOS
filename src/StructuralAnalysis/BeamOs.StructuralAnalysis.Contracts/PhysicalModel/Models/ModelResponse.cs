using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

public record ModelInfoResponse(
    Guid Id,
    string Name,
    string Description,
    ModelSettings Settings,
    DateTimeOffset LastModified,
    string Role
) : IBeamOsEntityResponse;

public record ModelResponse(
    Guid Id,
    string Name,
    string Description,
    ModelSettings Settings,
    DateTimeOffset LastModified,
    // Dictionary<string, object>? Metadata = null,
    List<NodeResponse>? Nodes = null,
    List<InternalNode>? InternalNodes = null,
    List<Element1dResponse>? Element1ds = null,
    List<MaterialResponse>? Materials = null,
    List<SectionProfileResponse>? SectionProfiles = null,
    List<SectionProfileFromLibrary>? SectionProfilesFromLibrary = null,
    List<PointLoadResponse>? PointLoads = null,
    List<MomentLoadResponse>? MomentLoads = null,
    List<ResultSetResponse>? ResultSets = null,
    List<LoadCase>? LoadCases = null,
    List<LoadCombination>? LoadCombinations = null
) : IBeamOsEntityResponse;

public record ModelResponseHydrated(
    Guid Id,
    string Name,
    string Description,
    ModelSettings Settings,
    List<NodeResponse> Nodes,
    List<Element1dResponse> Element1ds,
    List<MaterialResponse> Materials,
    List<SectionProfileResponse> SectionProfiles,
    List<PointLoadResponse> PointLoads,
    List<MomentLoadResponse> MomentLoads,
    List<ResultSetResponse> ResultSets
) : IBeamOsEntityResponse;

public record ModelSettings
{
    public required UnitSettings UnitSettings { get; init; }
    public required AnalysisSettings AnalysisSettings { get; init; }
    public required bool YAxisUp { get; init; }

    public ModelSettings() { }

    // [JsonConstructor]
    [SetsRequiredMembers]
    public ModelSettings(
        UnitSettings unitSettings,
        AnalysisSettings? analysisSettings = null,
        bool yAxisUp = true
    )
    {
        this.UnitSettings = unitSettings;
        this.AnalysisSettings = analysisSettings ?? new();
        this.YAxisUp = yAxisUp;
    }
}

public record AnalysisSettings
{
    public Element1dAnalysisType Element1DAnalysisType { get; set; } =
        Element1dAnalysisType.Timoshenko;

    public AnalysisSettings(Element1dAnalysisType? element1DAnalysisType)
    {
        this.Element1DAnalysisType = element1DAnalysisType ?? Element1dAnalysisType.Timoshenko;
    }

    public AnalysisSettings()
        : this(Element1dAnalysisType.Timoshenko) { }
}

public enum Element1dAnalysisType
{
    Undefined = 0,
    Euler = 1,
    Timoshenko = 2,
}
