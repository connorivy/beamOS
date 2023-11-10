namespace BeamOS.PhysicalModel.Contracts.Model;

public record CreateModelRequest(
    string Name,
    string Description,
    AnalyticalModelSettingsRequest Settings);

public record AnalyticalModelSettingsRequest(
    UnitSettingsRequest UnitSettings);

public record UnitSettingsRequest(
    string LengthUnit,
    string AreaUnit,
    string VolumeUnit,
    string ForceUnit,
    string ForcePerLengthUnit,
    string TorqueUnit);
