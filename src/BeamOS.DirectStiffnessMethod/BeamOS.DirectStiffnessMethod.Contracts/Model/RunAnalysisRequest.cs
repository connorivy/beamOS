namespace BeamOS.DirectStiffnessMethod.Contracts.Model;

public record RunAnalysisRequest(string ModelId);

public record ModelSettingsRequest(UnitSettingsRequest UnitSettings);

public record UnitSettingsRequest(
    string LengthUnit,
    string AreaUnit,
    string VolumeUnit,
    string ForceUnit,
    string ForcePerLengthUnit,
    string TorqueUnit
);
