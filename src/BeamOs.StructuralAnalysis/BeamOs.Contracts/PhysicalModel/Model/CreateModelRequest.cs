using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.Model;

public record CreateModelRequest(
    string Name,
    string Description,
    PhysicalModelSettings Settings,
    string? Id = null
);

public record PhysicalModelSettings
{
    public required UnitSettingsDtoVerbose UnitSettings { get; init; }
    public AnalysisSettingsContract AnalysisSettings { get; init; }
    public bool YAxisUp { get; init; }

    [JsonConstructor]
    [SetsRequiredMembers]
    public PhysicalModelSettings(
        UnitSettingsDtoVerbose unitSettings,
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
    public Element1dAnalysisType Element1DAnalysisType { get; set; }

    public AnalysisSettingsContract(Element1dAnalysisType element1DAnalysisType)
    {
        this.Element1DAnalysisType = element1DAnalysisType;
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
