using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Application.AnalyticalNodes.Commands;

public record CreateAnalyticalNodeCommand(
    string Id,
    string ModelId,
    PointCommand LocationPoint,
    RestraintCommand Restraint);

public record PointCommand(
    Length XCoordinate,
    Length YCoordinate,
    Length ZCoordinate);

public record RestraintCommand(
    bool CanTranslateAlongX,
    bool CanTranslateAlongY,
    bool CanTranslateAlongZ,
    bool CanRotateAboutX,
    bool CanRotateAboutY,
    bool CanRotateAboutZ);
