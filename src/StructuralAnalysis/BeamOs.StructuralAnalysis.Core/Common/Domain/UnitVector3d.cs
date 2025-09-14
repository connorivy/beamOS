using BeamOs.Common.Domain.Models;
using MathNet.Spatial.Euclidean;

namespace BeamOs.StructuralAnalysis.Domain.Common;

internal class UnitVector3d : BeamOSValueObject
{
    public UnitVector3d(double x, double y, double z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public UnitVector3d(UnitVector3D unitVector3D)
    {
        this.X = unitVector3D.X;
        this.Y = unitVector3D.Y;
        this.Z = unitVector3D.Z;
    }

    public double X { get; private set; }
    public double Y { get; private set; }
    public double Z { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.X;
        yield return this.Y;
        yield return this.Z;
    }
}

internal class Vector3d : BeamOSValueObject
{
    public Vector3d(double x, double y, double z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public Vector3d(Vector3D unitVector3D)
        : this(unitVector3D.X, unitVector3D.Y, unitVector3D.Z) { }

    public double X { get; private set; }
    public double Y { get; private set; }
    public double Z { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.X;
        yield return this.Y;
        yield return this.Z;
    }

    [Obsolete("EF Core Constructor", true)]
    protected Vector3d() { }
}
