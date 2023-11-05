namespace BeamOS.Common.Domain.ValueObjects;

/// <summary>
/// Describes the degrees of freedom of an element or node. A true value denotes free movement
/// while a false value denotes restraint
/// </summary>
public class Restraints : CoordinateDirectionBase<bool>
{
    public Restraints(
        bool canTranslateAlongX,
        bool canTranslateAlongY,
        bool canTranslateAlongZ,
        bool canRotationAboutX,
        bool canRotationAboutY,
        bool canRotationAboutZ) : base(
            canTranslateAlongX,
            canTranslateAlongY,
            canTranslateAlongZ,
            canRotationAboutX,
            canRotationAboutY,
            canRotationAboutZ)
    {
    }

    public bool CanTranslateAlongX => this.AlongX;
    public bool CanTranslateAlongY => this.AlongY;
    public bool CanTranslateAlongZ => this.AlongZ;
    public bool CanRotationAboutX => this.AboutX;
    public bool CanRotationAboutY => this.AboutY;
    public bool CanRotationAboutZ => this.AboutZ;
    public static Restraints Free { get; } = new(true, true, true, true, true, true);
    public static Restraints Fixed { get; } = new(false, false, false, false, false, false);
}
