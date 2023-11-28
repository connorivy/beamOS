namespace BeamOS.Common.Domain.ValueObjects;

/// <summary>
/// Describes the degrees of freedom of an element or node. A true value denotes free movement
/// while a false value denotes restraint
/// </summary>
public class Restraint(
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
    public bool CanTranslateAlongX { get => this.AlongX; private set => this.AlongX = value; }
    public bool CanTranslateAlongY { get => this.AlongY; private set => this.AlongY = value; }
    public bool CanTranslateAlongZ { get => this.AlongZ; private set => this.AlongZ = value; }
    public bool CanRotateAboutX { get => this.AboutX; private set => this.AboutX = value; }
    public bool CanRotateAboutY { get => this.AboutY; private set => this.AboutY = value; }
    public bool CanRotateAboutZ { get => this.AboutZ; private set => this.AboutZ = value; }
    public static Restraint Free { get; } = new(true, true, true, true, true, true);
    public static Restraint Fixed { get; } = new(false, false, false, false, false, false);
}
