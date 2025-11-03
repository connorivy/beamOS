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
    public AnalysisSettings AnalysisSettings { get; init; } = new();
    public WorkflowSettings WorkflowSettings
    {
        get;
        init
        {
            if (value is not null)
            {
                field = value;
            }
        }
    }
    public bool YAxisUp { get; init; }

    [JsonConstructor]
    public ModelSettings()
    {
        this.WorkflowSettings = new() { ModelingMode = ModelingMode.BimFirst };
    }

    // [JsonConstructor]
    [SetsRequiredMembers]
    public ModelSettings(
        UnitSettings unitSettings,
        AnalysisSettings? analysisSettings = null,
        WorkflowSettings? workflowSettings = null,
        bool yAxisUp = true
    )
    {
        this.UnitSettings = unitSettings;
        this.AnalysisSettings = analysisSettings ?? new();
        this.WorkflowSettings = workflowSettings ?? new() { ModelingMode = ModelingMode.BimFirst };
        this.YAxisUp = yAxisUp;
    }
}

public record WorkflowSettings
{
    public required ModelingMode ModelingMode { get; init; }

    /// <summary>
    /// The ID of the BIM source model, if applicable. This only applies when the ModelingMode is BimFirst.
    /// If the BimSourceModelId is set, it indicates that this model is linked to a specific BIM model.
    /// If it is null, it indicates that a new BIM source model should be created.
    /// </summary>
    public Guid? BimSourceModelId { get; init; }

    /// <summary>
    /// If the ModelingMode is BimFirst_BimSource, this list contains the IDs of all models that are
    /// configured as BIM First models linked to this BIM source model.
    /// </summary>
    public IList<Guid>? BimFirstModelIds { get; init; }

    public WorkflowSettings() { }
}

public enum ModelingMode : byte
{
    Undefined = 0,
    BimFirst = 1,
    Independent = 2,
    BimFirstSource = 3,
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
