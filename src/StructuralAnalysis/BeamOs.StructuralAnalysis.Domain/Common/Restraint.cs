using BeamOs.StructuralAnalysis.Domain.Common.Models;
using MathNet.Spatial.Euclidean;

namespace BeamOs.StructuralAnalysis.Domain.Common;

public class Restraint(
    bool canTranslateAlongX,
    bool canTranslateAlongY,
    bool canTranslateAlongZ,
    bool canRotateAboutX,
    bool canRotateAboutY,
    bool canRotateAboutZ
) : BeamOSValueObject
{
    public bool CanTranslateAlongX { get; private set; } = canTranslateAlongX;
    public bool CanTranslateAlongY { get; private set; } = canTranslateAlongY;
    public bool CanTranslateAlongZ { get; private set; } = canTranslateAlongZ;
    public bool CanRotateAboutX { get; private set; } = canRotateAboutX;
    public bool CanRotateAboutY { get; private set; } = canRotateAboutY;
    public bool CanRotateAboutZ { get; private set; } = canRotateAboutZ;

    public bool IsFullyRestrainedInDirection(Vector3D direction)
    {
        if (direction.X > .001 && this.CanTranslateAlongX)
        {
            return false;
        }
        if (direction.Y > .001 && this.CanTranslateAlongY)
        {
            return false;
        }
        if (direction.Z > .001 && this.CanTranslateAlongZ)
        {
            return false;
        }

        return true;
    }

    public bool IsFullyRestrainedAboutDirection(Vector3D direction)
    {
        if (direction.X > .001 && this.CanRotateAboutX)
        {
            return false;
        }
        if (direction.Y > .001 && this.CanRotateAboutY)
        {
            return false;
        }
        if (direction.Z > .001 && this.CanRotateAboutZ)
        {
            return false;
        }

        return true;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.CanTranslateAlongX;
        yield return this.CanTranslateAlongY;
        yield return this.CanTranslateAlongZ;
        yield return this.CanRotateAboutX;
        yield return this.CanRotateAboutY;
        yield return this.CanRotateAboutZ;
    }

    public static Restraint Free { get; } = new(true, true, true, true, true, true);
    public static Restraint FreeInXyPlane { get; } = new(true, true, false, false, false, true);
    public static Restraint Pinned { get; } = new(false, false, false, true, true, true);
    public static Restraint PinnedInXyPlane { get; } = new(false, false, false, false, false, true);
    public static Restraint Fixed { get; } = new(false, false, false, false, false, false);
}
