using BeamOs.Contracts.AnalyticalModel.AnalyticalNode;

namespace BeamOs.Contracts.AnalyticalModel.Results;

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
    List<NodeResultResponse> NodeResponses
//List<string> Element1DIds,
//List<string> MaterialIds,
//List<string> SectionProfileIds
);

public record AnalyticalModelResponse3(
    List<NodeResultResponse> NodeResponses
//List<string> Element1DIds,
//List<string> MaterialIds,
//List<string> SectionProfileIds
);

public record UnsupportedStructureDisplacementIdResponse(string AnalyticalNodeId, string Direction);
