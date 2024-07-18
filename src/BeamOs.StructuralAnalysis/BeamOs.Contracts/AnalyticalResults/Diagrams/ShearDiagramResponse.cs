using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.AnalyticalResults.Diagrams;

public record ShearDiagramResponse(
    string Id,
    string Element1DId,
    Vector3 GlobalShearDirection,
    string LengthUnit,
    string ForceUnit,
    UnitValueDto ElementLength,
    DiagramConsistantIntervalResponse[] Intervals
);
