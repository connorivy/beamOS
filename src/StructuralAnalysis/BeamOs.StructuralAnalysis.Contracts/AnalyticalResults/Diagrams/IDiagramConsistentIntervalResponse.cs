using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;

public interface IDiagramConsistentIntervalResponse
{
    LengthContract EndLocation { get; init; }
    double[] PolynomialCoefficients { get; init; }
    LengthContract StartLocation { get; init; }
}
