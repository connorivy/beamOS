namespace BeamOS.Common.Domain.ValueObjects;

/// <summary>
/// Describes the degrees of freedom of an element or node. A true value denotes free movement
/// while a false value denotes restraint
/// </summary>
public class Restraints(
    bool canTranslateAlongX,
    bool canTranslateAlongY,
    bool canTranslateAlongZ,
    bool canRotateAboutX,
    bool canRotateAboutY,
    bool canRotateAboutZ) : CoordinateDirectionBase<bool>(
        canTranslateAlongX,
        canTranslateAlongY,
        canTranslateAlongZ,
        canRotateAboutX,
        canRotateAboutY,
        canRotateAboutZ)
{
    public bool CanTranslateAlongX => this.AlongX;
    public bool CanTranslateAlongY => this.AlongY;
    public bool CanTranslateAlongZ => this.AlongZ;
    public bool CanRotateAboutX => this.AboutX;
    public bool CanRotateAboutY => this.AboutY;
    public bool CanRotateAboutZ => this.AboutZ;
    public static Restraints Free { get; } = new(true, true, true, true, true, true);
    public static Restraints Fixed { get; } = new(false, false, false, false, false, false);
}
