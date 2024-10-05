using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.AnalyticalModel.Diagrams;

public record DiagramResponse(
    string Id,
    string Element1DId,
    string LengthUnit,
    string DiagramValueUnit,
    LengthContract ElementLength,
    DiagramConsistentIntervalResponse[] Intervals
);

public record DiagramConsistentIntervalResponse(
    LengthContract StartLocation,
    LengthContract EndLocation,
    double[] PolynomialCoefficients
);
