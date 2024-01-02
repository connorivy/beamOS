namespace BeamOS.DirectStiffnessMethod.Contracts.Model;

public record AnalyticalModelResponse(
    List<UnsupportedStructureDisplacementIdResponse> DegreeOfFreedomIds,
    List<UnsupportedStructureDisplacementIdResponse> BoundaryConditionIds,
    List<double> AnalyticalNodeDisplacements,
    List<double> AnalyticalNodeReactions
//List<string> Element1DIds,
//List<string> MaterialIds,
//List<string> SectionProfileIds
);

public record UnsupportedStructureDisplacementIdResponse(string AnalyticalNodeId, string Direction);

public record ModelSettingsResponse(UnitSettingsResponse UnitSettings);

public record UnitSettingsResponse(
    string LengthUnit,
    string AreaUnit,
    string VolumeUnit,
    string ForceUnit,
    string ForcePerLengthUnit,
    string TorqueUnit
);
