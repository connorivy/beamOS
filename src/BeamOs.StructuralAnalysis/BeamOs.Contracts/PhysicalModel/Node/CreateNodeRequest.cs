using System.ComponentModel.DataAnnotations;

namespace BeamOs.Contracts.PhysicalModel.Node;

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
)
{
    public static RestraintsRequest Free { get; } = new(true, true, true, true, true, true);
    public static RestraintsRequest Fixed { get; } = new(false, false, false, false, false, false);
}
