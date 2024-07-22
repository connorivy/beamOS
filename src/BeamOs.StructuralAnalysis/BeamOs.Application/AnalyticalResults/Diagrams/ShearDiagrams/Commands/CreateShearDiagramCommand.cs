using BeamOs.Domain.Common.Enums;

namespace BeamOs.Application.AnalyticalResults.Diagrams.ShearDiagrams.Commands;

public record CreateShearDiagramCommand(
    string Element1dId,
    LinearCoordinateDirection3D LinearCoordinateDirection3D
);
