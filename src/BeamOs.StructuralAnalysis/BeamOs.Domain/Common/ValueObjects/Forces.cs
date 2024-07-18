using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.Common.ValueObjects;

public sealed class Forces : CoordinateDirectionBase<Force, Torque>
{
    public Forces(
        Force forceAlongX,
        Force forceAlongY,
        Force forceAlongZ,
        Torque momentAboutX,
        Torque momentAboutY,
        Torque momentAboutZ
    )
        : base(forceAlongX, forceAlongY, forceAlongZ, momentAboutX, momentAboutY, momentAboutZ) { }

    public Forces(
        double forceAlongX,
        double forceAlongY,
        double forceAlongZ,
        double momentAboutX,
        double momentAboutY,
        double momentAboutZ,
        ForceUnit forceUnit,
        TorqueUnit torqueUnit
    )
        : this(
            new(forceAlongX, forceUnit),
            new(forceAlongY, forceUnit),
            new(forceAlongZ, forceUnit),
            new(momentAboutX, torqueUnit),
            new(momentAboutY, torqueUnit),
            new(momentAboutZ, torqueUnit)
        ) { }

    public Force ForceAlongX
    {
        get => this.AlongX;
        private set => this.AlongX = value;
    }
    public Force ForceAlongY
    {
        get => this.AlongY;
        private set => this.AlongY = value;
    }
    public Force ForceAlongZ
    {
        get => this.AlongZ;
        private set => this.AlongZ = value;
    }
    public Torque MomentAboutX
    {
        get => this.AboutX;
        private set => this.AboutX = value;
    }
    public Torque MomentAboutY
    {
        get => this.AboutY;
        private set => this.AboutY = value;
    }
    public Torque MomentAboutZ
    {
        get => this.AboutZ;
        private set => this.AboutZ = value;
    }

    public Force GetForceInDirection(Vector3D direction) =>
        this.GetForceInDirection(direction.Normalize());

    public Force GetForceInDirection(UnitVector3D direction)
    {
        return this.ForceAlongX * direction.X
            + this.ForceAlongY * direction.Y
            + this.ForceAlongZ * direction.Z;
    }

    public double[] ToArray(ForceUnit forceUnit, TorqueUnit torqueUnit)
    {
        return
        [
            this.ForceAlongX.As(forceUnit),
            this.ForceAlongY.As(forceUnit),
            this.ForceAlongZ.As(forceUnit),
            this.MomentAboutX.As(torqueUnit),
            this.MomentAboutY.As(torqueUnit),
            this.MomentAboutZ.As(torqueUnit),
        ];
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.ForceAlongX;
        yield return this.ForceAlongY;
        yield return this.ForceAlongZ;
        yield return this.MomentAboutX;
        yield return this.MomentAboutY;
        yield return this.MomentAboutZ;
    }
}
