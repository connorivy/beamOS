namespace BeamOS.PhysicalModel.Contracts.Nodes;

public record CreateNodeRequest(
    string ModelId,
    double XCoordinate,
    double YCoordinate,
    double ZCoordinate,
    string? LengthUnit = null,
    RestraintsRequest? Restraint = null);

public record RestraintsRequest(
    bool CanTranslateAlongX,
    bool CanTranslateAlongY,
    bool CanTranslateAlongZ,
    bool CanRotateAboutX,
    bool CanRotateAboutY,
    bool CanRotateAboutZ);
