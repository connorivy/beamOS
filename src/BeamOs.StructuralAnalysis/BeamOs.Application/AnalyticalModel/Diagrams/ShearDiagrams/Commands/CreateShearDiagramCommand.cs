using BeamOs.Domain.Common.Enums;

namespace BeamOs.Application.AnalyticalModel.Diagrams.ShearDiagrams.Commands;

public record CreateShearDiagramCommand(
    string Element1dId,
    LinearCoordinateDirection3D LinearCoordinateDirection3D
);
