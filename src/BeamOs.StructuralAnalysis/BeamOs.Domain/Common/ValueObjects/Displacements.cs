using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.Common.ValueObjects;

public sealed class Displacements : CoordinateDirectionBase<Length, Angle>
{
    public Displacements(
        Length displacementAlongX,
        Length displacementAlongY,
        Length displacementAlongZ,
        Angle rotationAboutX,
        Angle rotationAboutY,
        Angle rotationAboutZ
    )
        : base(
            displacementAlongX,
            displacementAlongY,
            displacementAlongZ,
            rotationAboutX,
            rotationAboutY,
            rotationAboutZ
        ) { }

    public Length DisplacementAlongX => this.AlongX;
    public Length DisplacementAlongY => this.AlongY;
    public Length DisplacementAlongZ => this.AlongZ;
    public Angle RotationAboutX => this.AboutX;
    public Angle RotationAboutY => this.AboutY;
    public Angle RotationAboutZ => this.AboutZ;

    public double[] ToArray(LengthUnit lengthUnit, AngleUnit angleUnit)
    {
        return
        [
            this.DisplacementAlongX.As(lengthUnit),
            this.DisplacementAlongY.As(lengthUnit),
            this.DisplacementAlongZ.As(lengthUnit),
            this.RotationAboutX.As(angleUnit),
            this.RotationAboutY.As(angleUnit),
            this.RotationAboutZ.As(angleUnit),
        ];
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.DisplacementAlongX;
        yield return this.DisplacementAlongY;
        yield return this.DisplacementAlongZ;
        yield return this.RotationAboutX;
        yield return this.RotationAboutY;
        yield return this.RotationAboutZ;
    }
}
