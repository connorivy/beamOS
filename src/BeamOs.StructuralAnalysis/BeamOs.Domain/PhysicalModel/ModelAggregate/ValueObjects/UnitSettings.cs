using BeamOs.Domain.Common.Models;
using UnitsNet.Units;

namespace BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

public class UnitSettings : BeamOSValueObject
{
    public UnitSettings(
        LengthUnit lengthUnit,
        AreaUnit areaUnit,
        VolumeUnit volumeUnit,
        ForceUnit forceUnit,
        ForcePerLengthUnit forcePerLengthUnit,
        TorqueUnit torqueUnit,
        PressureUnit pressureUnit,
        AreaMomentOfInertiaUnit areaMomentOfInertiaUnit
    )
    {
        this.LengthUnit = lengthUnit;
        this.AreaUnit = areaUnit;
        this.VolumeUnit = volumeUnit;
        this.ForceUnit = forceUnit;
        this.ForcePerLengthUnit = forcePerLengthUnit;
        this.TorqueUnit = torqueUnit;
        this.PressureUnit = pressureUnit;
        this.AreaMomentOfInertiaUnit = areaMomentOfInertiaUnit;
    }

    public LengthUnit LengthUnit { get; private set; }
    public AreaUnit AreaUnit { get; private set; }
    public VolumeUnit VolumeUnit { get; private set; }
    public ForceUnit ForceUnit { get; private set; }
    public ForcePerLengthUnit ForcePerLengthUnit { get; private set; }
    public TorqueUnit TorqueUnit { get; private set; }
    public AngleUnit AngleUnit { get; private set; } = AngleUnit.Radian; // TODO : support degrees
    public PressureUnit PressureUnit { get; private set; }
    public AreaMomentOfInertiaUnit AreaMomentOfInertiaUnit { get; private set; }

    public TUnitType GetUnit<TUnitType>()
        where TUnitType : Enum
    {
        foreach (var propInfo in typeof(UnitSettings).GetProperties())
        {
            var type = propInfo.PropertyType;
            if (typeof(TUnitType) != type)
            {
                continue;
            }

            var prop = propInfo.GetValue(this, null);
            if (prop is not TUnitType unitType)
            {
                throw new Exception("This shouldn't happen");
            }

            return unitType;
        }

        throw new Exception(
            $"Unit of type {typeof(TUnitType).FullName} is not defined by UnitSettings"
        );
    }

    public Enum GetUnit(Type unitType)
    {
        foreach (var propInfo in typeof(UnitSettings).GetProperties())
        {
            var type = propInfo.PropertyType;
            if (unitType != type)
            {
                continue;
            }

            return (Enum)propInfo.GetValue(this, null);
        }

        throw new Exception($"Unit of type {unitType.FullName} is not defined by UnitSettings");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.LengthUnit;
        yield return this.AreaUnit;
        yield return this.VolumeUnit;
        yield return this.ForceUnit;
        yield return this.ForcePerLengthUnit;
        yield return this.TorqueUnit;
    }

    public static UnitSettings SI { get; } =
        new(
            LengthUnit.Meter,
            AreaUnit.SquareMeter,
            VolumeUnit.CubicMeter,
            ForceUnit.Newton,
            ForcePerLengthUnit.NewtonPerMeter,
            TorqueUnit.NewtonMeter,
            PressureUnit.NewtonPerSquareMeter,
            AreaMomentOfInertiaUnit.MeterToTheFourth
        );
    public static UnitSettings K_IN { get; } =
        new(
            LengthUnit.Inch,
            AreaUnit.SquareInch,
            VolumeUnit.CubicInch,
            ForceUnit.KilopoundForce,
            ForcePerLengthUnit.KilopoundForcePerInch,
            TorqueUnit.KilopoundForceInch,
            PressureUnit.KilopoundForcePerSquareInch,
            AreaMomentOfInertiaUnit.InchToTheFourth
        );
    public static UnitSettings K_FT { get; } =
        new(
            LengthUnit.Foot,
            AreaUnit.SquareFoot,
            VolumeUnit.CubicFoot,
            ForceUnit.KilopoundForce,
            ForcePerLengthUnit.KilopoundForcePerFoot,
            TorqueUnit.KilopoundForceFoot,
            PressureUnit.KilopoundForcePerSquareFoot,
            AreaMomentOfInertiaUnit.FootToTheFourth
        );

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private UnitSettings() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
