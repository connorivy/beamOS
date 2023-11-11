namespace BeamOS.PhysicalModel.Contracts.Model;
public record ModelResponse(
    string Id,
    string Name,
    string Description,
    ModelSettingsResponse Settings,
    List<string> AnalyticalNodeIds,
    List<string> Element1DIds,
    List<string> MaterialIds,
    List<string> SectionProfileIds);

public record ModelSettingsResponse(
    UnitSettingsResponse UnitSettings);

public record UnitSettingsResponse(
    string LengthUnit,
    string AreaUnit,
    string VolumeUnit,
    string ForceUnit,
    string ForcePerLengthUnit,
    string TorqueUnit);
