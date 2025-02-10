using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;

public record DiagramResponse
{
    public Guid ModelId { get; init; }
    public int ResultSetId { get; init; }
    public int Element1dId { get; init; }
    public DiagramConsistentIntervalResponse[] Intervals { get; init; }

    public DiagramResponse(
        Guid modelId,
        int resultSetId,
        int element1dId,
        DiagramConsistentIntervalResponse[] intervals
    )
    {
        this.ModelId = modelId;
        this.ResultSetId = resultSetId;
        this.Element1dId = element1dId;
        this.Intervals = intervals;
    }
}

public record DiagramConsistentIntervalResponse(
    LengthContract StartLocation,
    LengthContract EndLocation,
    double[] PolynomialCoefficients
);

public record ShearDiagramResponse(
    Guid ModelId,
    int ResultSetId,
    int Element1DId,
    Vector3 GlobalShearDirection,
    LengthUnitContract LengthUnit,
    ForceUnitContract ForceUnit,
    LengthContract ElementLength,
    DiagramConsistentIntervalResponse[] Intervals
) : DiagramResponse(ModelId, ResultSetId, Element1DId, Intervals);

public record MomentDiagramResponse(
    Guid ModelId,
    int ResultSetId,
    int Element1DId,
    LengthUnitContract LengthUnit,
    TorqueUnitContract TorqueUnit,
    LengthContract ElementLength,
    DiagramConsistentIntervalResponse[] Intervals
) : DiagramResponse(ModelId, ResultSetId, Element1DId, Intervals);
