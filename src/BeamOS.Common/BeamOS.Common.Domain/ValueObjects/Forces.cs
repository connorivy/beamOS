using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.Common.Domain.ValueObjects;

public class Forces : CoordinateDirectionBase<Force, Torque>
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

    public Force ForceAlongX => this.AlongX;
    public Force ForceAlongY => this.AlongY;
    public Force ForceAlongZ => this.AlongZ;
    public Torque MomentAboutX => this.AboutX;
    public Torque MomentAboutY => this.AboutY;
    public Torque MomentAboutZ => this.AboutZ;

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
