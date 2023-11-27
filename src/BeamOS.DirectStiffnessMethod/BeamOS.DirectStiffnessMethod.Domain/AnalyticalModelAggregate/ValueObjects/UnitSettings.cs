using BeamOS.Common.Domain.Models;
using UnitsNet.Units;

namespace BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate.ValueObjects;
public class UnitSettings : BeamOSValueObject
{
    public UnitSettings(
        LengthUnit lengthUnit,
        AreaUnit areaUnit,
        VolumeUnit volumeUnit,
        ForceUnit forceUnit,
        ForcePerLengthUnit forcePerLengthUnit,
        TorqueUnit torqueUnit
    )
    {
        this.LengthUnit = lengthUnit;
        this.AreaUnit = areaUnit;
        this.VolumeUnit = volumeUnit;
        this.ForceUnit = forceUnit;
        this.ForcePerLengthUnit = forcePerLengthUnit;
        this.TorqueUnit = torqueUnit;
    }

    public LengthUnit LengthUnit { get; }
    public AreaUnit AreaUnit { get; }
    public VolumeUnit VolumeUnit { get; }
    public ForceUnit ForceUnit { get; }
    public ForcePerLengthUnit ForcePerLengthUnit { get; }
    public TorqueUnit TorqueUnit { get; }
    public static UnitSettings SI { get; } = new(
        LengthUnit.Meter,
        AreaUnit.SquareMeter,
        VolumeUnit.CubicMeter,
        ForceUnit.Newton,
        ForcePerLengthUnit.NewtonPerMeter,
        TorqueUnit.NewtonMeter);
    public static UnitSettings K_IN { get; } = new(
        LengthUnit.Inch,
        AreaUnit.SquareInch,
        VolumeUnit.CubicInch,
        ForceUnit.KilopoundForce,
        ForcePerLengthUnit.KilopoundForcePerInch,
        TorqueUnit.KilopoundForceInch);
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.LengthUnit;
        yield return this.AreaUnit;
        yield return this.VolumeUnit;
        yield return this.ForceUnit;
        yield return this.ForcePerLengthUnit;
        yield return this.TorqueUnit;
    }
}
