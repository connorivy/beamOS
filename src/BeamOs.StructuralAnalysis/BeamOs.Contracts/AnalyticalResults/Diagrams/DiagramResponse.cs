using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.AnalyticalResults.Diagrams;

public record DiagramResponse(
    string Id,
    string Element1DId,
    string LengthUnit,
    string DiagramValueUnit,
    UnitValueDto ElementLength,
    DiagramConsistantIntervalResponse[] Intervals
);

public record DiagramConsistantIntervalResponse(
    UnitValueDto StartLocation,
    UnitValueDto EndLocation,
    double[] PolynomialCoefficients
);
