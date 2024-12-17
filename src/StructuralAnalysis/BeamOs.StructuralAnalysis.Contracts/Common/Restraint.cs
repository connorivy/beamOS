using System.Diagnostics.CodeAnalysis;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

public readonly record struct Restraint
{
    public Restraint() { }

    [SetsRequiredMembers]
    public Restraint(
        bool canTranslateAlongX,
        bool canTranslateAlongY,
        bool canTranslateAlongZ,
        bool canRotateAboutX,
        bool canRotateAboutY,
        bool canRotateAboutZ
    )
    {
        this.CanTranslateAlongX = canTranslateAlongX;
        this.CanTranslateAlongY = canTranslateAlongY;
        this.CanTranslateAlongZ = canTranslateAlongZ;
        this.CanRotateAboutX = canRotateAboutX;
        this.CanRotateAboutY = canRotateAboutY;
        this.CanRotateAboutZ = canRotateAboutZ;
    }

    public required bool CanTranslateAlongX { get; init; }
    public required bool CanTranslateAlongY { get; init; }
    public required bool CanTranslateAlongZ { get; init; }
    public required bool CanRotateAboutX { get; init; }
    public required bool CanRotateAboutY { get; init; }
    public required bool CanRotateAboutZ { get; init; }

    public static Restraint Free { get; } = new(true, true, true, true, true, true);
    public static Restraint FreeXzPlane { get; } = new(true, false, true, false, true, false);
    public static Restraint FreeXyPlane { get; } = new(true, true, false, false, false, true);
    public static Restraint Pinned { get; } = new(false, false, false, true, true, true);
    public static Restraint Fixed { get; } = new(false, false, false, false, false, false);
}

public record struct PartialRestraint(
    bool? CanTranslateAlongX = null,
    bool? CanTranslateAlongY = null,
    bool? CanTranslateAlongZ = null,
    bool? CanRotateAboutX = null,
    bool? CanRotateAboutY = null,
    bool? CanRotateAboutZ = null
)
{
    public static implicit operator PartialRestraint(Restraint restraint) =>
        new(
            restraint.CanTranslateAlongY,
            restraint.CanTranslateAlongY,
            restraint.CanTranslateAlongZ,
            restraint.CanRotateAboutX,
            restraint.CanRotateAboutY,
            restraint.CanRotateAboutZ
        );
}
