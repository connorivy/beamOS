using System.ComponentModel.DataAnnotations;

namespace BeamOs.Contracts.PhysicalModel.Node;

public record CreateNodeRequest(
    [Required] string ModelId,
    double XCoordinate,
    double YCoordinate,
    double ZCoordinate,
    string? LengthUnit = null,
    RestraintRequest? Restraint = null
);

public record RestraintRequest(
    bool CanTranslateAlongX,
    bool CanTranslateAlongY,
    bool CanTranslateAlongZ,
    bool CanRotateAboutX,
    bool CanRotateAboutY,
    bool CanRotateAboutZ
)
{
    public static RestraintRequest Free { get; } = new(true, true, true, true, true, true);
    public static RestraintRequest Fixed { get; } = new(false, false, false, false, false, false);
}
