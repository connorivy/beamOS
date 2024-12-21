using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;

public record ModelResponse(
    Guid Id,
    string Name,
    string Description,
    PhysicalModelSettings Settings,
    List<NodeResponse>? Nodes = null
//List<Element1DResponse>? Element1ds = null,
//List<MaterialResponse>? Materials = null,
//List<SectionProfileResponse>? SectionProfiles = null,
//List<PointLoadResponse>? PointLoads = null,
//List<MomentLoadResponse>? MomentLoads = null,
//AnalyticalResultsResponse? AnalyticalResults = null
);

public record ModelSettingsResponse(UnitSettingsResponse UnitSettings);

public record UnitSettingsResponse(
    string LengthUnit,
    string AreaUnit,
    string VolumeUnit,
    string ForceUnit,
    string ForcePerLengthUnit,
    string TorqueUnit,
    string PressureUnit,
    string AreaMomentOfInertiaUnit
)
{
    public static UnitSettingsResponse K_IN { get; } =
        new(
            "Inch",
            "SquareInch",
            "CubicInch",
            "KilopoundForce",
            "KilopoundForcePerInch",
            "KilopoundForceInch",
            "KilopoundForcePerSquareInch",
            "InchToTheFourth"
        );
}

public record PhysicalModelSettings
{
    public required UnitSettingsContract UnitSettings { get; init; }
    public AnalysisSettingsContract AnalysisSettings { get; init; }
    public bool YAxisUp { get; init; }

    [JsonConstructor]
    [SetsRequiredMembers]
    public PhysicalModelSettings(
        UnitSettingsContract unitSettings,
        AnalysisSettingsContract? analysisSettings = null,
        bool yAxisUp = true
    )
    {
        this.UnitSettings = unitSettings;
        this.AnalysisSettings = analysisSettings ?? new();
        this.YAxisUp = yAxisUp;
    }
}

public record AnalysisSettingsContract
{
    public Element1dAnalysisType Element1DAnalysisType { get; set; } =
        Element1dAnalysisType.Timoshenko;

    public AnalysisSettingsContract(Element1dAnalysisType? element1DAnalysisType)
    {
        this.Element1DAnalysisType = element1DAnalysisType ?? Element1dAnalysisType.Timoshenko;
    }

    public AnalysisSettingsContract()
        : this(Element1dAnalysisType.Timoshenko) { }
}

public enum Element1dAnalysisType
{
    Undefined = 0,
    Euler = 1,
    Timoshenko = 2
}
