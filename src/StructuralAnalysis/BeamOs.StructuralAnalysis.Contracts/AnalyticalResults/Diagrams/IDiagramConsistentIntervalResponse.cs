using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;

public interface IDiagramConsistentIntervalResponse
{
    Length EndLocation { get; init; }
    double[] PolynomialCoefficients { get; init; }
    Length StartLocation { get; init; }
}
