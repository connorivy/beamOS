using BeamOS.Common.Domain.Models;
using UnitsNet.Units;

namespace BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
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

    public LengthUnit LengthUnit { get; private set; }
    public AreaUnit AreaUnit { get; private set; }
    public VolumeUnit VolumeUnit { get; private set; }
    public ForceUnit ForceUnit { get; private set; }
    public ForcePerLengthUnit ForcePerLengthUnit { get; private set; }
    public TorqueUnit TorqueUnit { get; private set; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.LengthUnit;
        yield return this.AreaUnit;
        yield return this.VolumeUnit;
        yield return this.ForceUnit;
        yield return this.ForcePerLengthUnit;
        yield return this.TorqueUnit;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private UnitSettings() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
