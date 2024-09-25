using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Common;

namespace BeamOs.Contracts.AnalyticalResults.Diagrams;

public record ShearDiagramResponse(
    string Id,
    string Element1DId,
    Vector3 GlobalShearDirection,
    string LengthUnit,
    string ForceUnit,
    LengthContract ElementLength,
    DiagramConsistentIntervalResponse[] Intervals
) : BeamOsEntityContractBase(Id);

public record MomentDiagramResponse(
    string Id,
    string Element1DId,
    string LengthUnit,
    string ForceUnit,
    LengthContract ElementLength,
    DiagramConsistentIntervalResponse[] Intervals
) : BeamOsEntityContractBase(Id);
