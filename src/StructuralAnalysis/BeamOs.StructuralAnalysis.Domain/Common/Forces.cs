using BeamOs.Common.Domain.Models;
using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.Common;

public sealed class Forces : BeamOSValueObject
{
    public Forces(
        Force forceAlongX,
        Force forceAlongY,
        Force forceAlongZ,
        Torque momentAboutX,
        Torque momentAboutY,
        Torque momentAboutZ
    )
    {
        this.ForceAlongX = forceAlongX;
        this.ForceAlongY = forceAlongY;
        this.ForceAlongZ = forceAlongZ;
        this.MomentAboutX = momentAboutX;
        this.MomentAboutY = momentAboutY;
        this.MomentAboutZ = momentAboutZ;
    }

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

    public Force ForceAlongX { get; private set; }
    public Force ForceAlongY { get; private set; }
    public Force ForceAlongZ { get; private set; }
    public Torque MomentAboutX { get; private set; }
    public Torque MomentAboutY { get; private set; }
    public Torque MomentAboutZ { get; private set; }

    public Force GetForceInDirection(Vector3D direction) =>
        this.GetForceInDirection(direction.Normalize());

    public Force GetForceInDirection(UnitVector3D direction)
    {
        return this.ForceAlongX * direction.X
            + this.ForceAlongY * direction.Y
            + this.ForceAlongZ * direction.Z;
    }

    public Torque GetTorqueAboutAxis(Vector3D axis) => this.GetTorqueAboutAxis(axis.Normalize());

    public Torque GetTorqueAboutAxis(UnitVector3D direction)
    {
        return this.MomentAboutX * direction.X
            + this.MomentAboutY * direction.Y
            + this.MomentAboutZ * direction.Z;
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
