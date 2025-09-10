using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;

public record DiagramResponseBase
{
    public Guid ModelId { get; init; }
    public int ResultSetId { get; init; }
    public int Element1dId { get; init; }
    public DiagramConsistentIntervalResponse[] Intervals { get; init; }

    public DiagramResponseBase(
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
    Length StartLocation,
    Length EndLocation,
    double[] PolynomialCoefficients
) : IDiagramConsistentIntervalResponse;

public record ShearDiagramResponse(
    Guid ModelId,
    int ResultSetId,
    int Element1dId,
    Vector3 GlobalShearDirection,
    LengthUnit LengthUnit,
    ForceUnit ForceUnit,
    Length ElementLength,
    DiagramConsistentIntervalResponse[] Intervals
) : DiagramResponseBase(ModelId, ResultSetId, Element1dId, Intervals);

public record MomentDiagramResponse(
    Guid ModelId,
    int ResultSetId,
    int Element1dId,
    LengthUnit LengthUnit,
    TorqueUnit TorqueUnit,
    Length ElementLength,
    DiagramConsistentIntervalResponse2[] Intervals
);

// weird bug with generating the openapi document if I use the same diagramConsistantIntervalResponse
// as the shear diagram response
public record DiagramConsistentIntervalResponse2(
    Length StartLocation,
    Length EndLocation,
    double[] PolynomialCoefficients
) : IDiagramConsistentIntervalResponse;

public readonly record struct DeflectionDiagramResponse
{
    public required int Element1dId { get; init; }
    public required int NumSteps { get; init; }
    public required double[] Offsets { get; init; }
}

public record AnalyticalResultsResponse : IModelEntity
{
    public ShearDiagramResponse[] ShearDiagrams { get; init; }
    public MomentDiagramResponse[] MomentDiagrams { get; init; }
    public DeflectionDiagramResponse[] DeflectionDiagrams { get; init; }
    public GlobalStresses GlobalStresses { get; init; }
    public required int Id { get; init; }
    public required Guid ModelId { get; init; }
}

public record DiagramResponse
{
    public required ShearDiagramResponse[] ShearDiagrams { get; init; }
    public required MomentDiagramResponse[] MomentDiagrams { get; init; }
    public required DeflectionDiagramResponse[] DeflectionDiagrams { get; init; }
}

public readonly record struct GlobalStresses
{
    public required Force MaxShear { get; init; }
    public required Force MinShear { get; init; }
    public required Torque MaxMoment { get; init; }
    public required Torque MinMoment { get; init; }
}

public enum DiagramType
{
    None = 0,
    Shear,
    Moment,
    Displacement
}

public enum RelativeDirection3D
{
    Undefined = 0,
    LocalX,
    LocalY,
    LocalZ,
    GlobalX,
    GlobalY,
    GlobalZ
}
