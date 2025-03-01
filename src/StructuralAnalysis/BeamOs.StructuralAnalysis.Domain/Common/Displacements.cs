using System.Diagnostics;
using BeamOs.Common.Domain.Models;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.Common;

public sealed class Displacements(
    Length displacementAlongX,
    Length displacementAlongY,
    Length displacementAlongZ,
    Angle rotationAboutX,
    Angle rotationAboutY,
    Angle rotationAboutZ
) : BeamOSValueObject
{
    public Displacements(
        double displacementAlongX,
        double displacementAlongY,
        double displacementAlongZ,
        double rotationAboutX,
        double rotationAboutY,
        double rotationAboutZ,
        LengthUnit lengthUnit,
        AngleUnit angleUnit
    )
        : this(
            new(displacementAlongX, lengthUnit),
            new(displacementAlongY, lengthUnit),
            new(displacementAlongZ, lengthUnit),
            new(rotationAboutX, angleUnit),
            new(rotationAboutY, angleUnit),
            new(rotationAboutZ, angleUnit)
        ) { }

    public Length DisplacementAlongX { get; private set; } = displacementAlongX;
    public Length DisplacementAlongY { get; private set; } = displacementAlongY;
    public Length DisplacementAlongZ { get; private set; } = displacementAlongZ;
    public Angle RotationAboutX { get; private set; } = rotationAboutX;
    public Angle RotationAboutY { get; private set; } = rotationAboutY;
    public Angle RotationAboutZ { get; private set; } = rotationAboutZ;

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

    public void CopyTo(Span<double> span, LengthUnit lengthUnit, AngleUnit angleUnit)
    {
        Debug.Assert(span.Length >= 6);

        span[0] = this.DisplacementAlongX.As(lengthUnit);
        span[1] = this.DisplacementAlongY.As(lengthUnit);
        span[2] = this.DisplacementAlongZ.As(lengthUnit);
        span[3] = this.RotationAboutX.As(angleUnit);
        span[4] = this.RotationAboutY.As(angleUnit);
        span[5] = this.RotationAboutZ.As(angleUnit);
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
