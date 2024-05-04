using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;

namespace BeamOs.Contracts.AnalyticalResults.Model;

public record AnalyticalModelResponse(
    List<UnsupportedStructureDisplacementIdResponse> DegreeOfFreedomIds,
    List<UnsupportedStructureDisplacementIdResponse> BoundaryConditionIds,
    List<double> AnalyticalNodeDisplacements,
    List<double> AnalyticalNodeReactions
//List<string> Element1DIds,
//List<string> MaterialIds,
//List<string> SectionProfileIds
);

public record AnalyticalModelResponse2(
    List<UnsupportedStructureDisplacementIdResponse> DegreeOfFreedomIds,
    List<UnsupportedStructureDisplacementIdResponse> BoundaryConditionIds,
    List<AnalyticalNodeResponse> NodeResponses
//List<string> Element1DIds,
//List<string> MaterialIds,
//List<string> SectionProfileIds
);

public record AnalyticalModelResponse3(
    List<AnalyticalNodeResponse> NodeResponses
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
