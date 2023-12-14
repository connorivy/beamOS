using System.ComponentModel.DataAnnotations;

namespace BeamOS.PhysicalModel.Contracts.Node;

public record CreateNodeRequest(
    [Required] string ModelId,
    double XCoordinate,
    double YCoordinate,
    double ZCoordinate,
    string? LengthUnit = null,
    RestraintsRequest? Restraint = null
);

public record RestraintsRequest(
    bool CanTranslateAlongX,
    bool CanTranslateAlongY,
    bool CanTranslateAlongZ,
    bool CanRotateAboutX,
    bool CanRotateAboutY,
    bool CanRotateAboutZ
);
