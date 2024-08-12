using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.Common;
using BeamOs.Domain.Common.ValueObjects;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Application.Common.Mappers;

public abstract class AbstractMapperProvidedUnits<TFrom, TTo>(
    LengthUnit lengthUnit,
    AreaUnit areaUnit,
    VolumeUnit volumeUnit,
    ForceUnit forceUnit,
    ForcePerLengthUnit forcePerLengthUnit,
    TorqueUnit torqueUnit,
    AngleUnit angleUnit,
    PressureUnit pressureUnit,
    AreaMomentOfInertiaUnit areaMomentOfInertiaUnit
) : AbstractMapper<TFrom, TTo>
{
    public AbstractMapperProvidedUnits(UnitSettings unitSettings)
        : this(
            unitSettings.LengthUnit,
            unitSettings.AreaUnit,
            unitSettings.VolumeUnit,
            unitSettings.ForceUnit,
            unitSettings.ForcePerLengthUnit,
            unitSettings.TorqueUnit,
            unitSettings.AngleUnit,
            unitSettings.PressureUnit,
            unitSettings.AreaMomentOfInertiaUnit
        ) { }

    protected LengthUnit LengthUnit { get; } = lengthUnit;
    protected AreaUnit AreaUnit { get; } = areaUnit;
    protected VolumeUnit VolumeUnit { get; } = volumeUnit;
    protected ForceUnit ForceUnit { get; } = forceUnit;
    protected ForcePerLengthUnit ForcePerLengthUnit { get; } = forcePerLengthUnit;
    protected TorqueUnit TorqueUnit { get; } = torqueUnit;
    protected AngleUnit AngleUnit { get; } = angleUnit;
    protected PressureUnit PressureUnit { get; } = pressureUnit;
    protected AreaMomentOfInertiaUnit AreaMomentOfInertiaUnit { get; } = areaMomentOfInertiaUnit;

    protected UnitValueDto UnitToUnitValueDtoMapper(Angle unit)
    {
        return new UnitValueDto(
            unit.As(this.AngleUnit),
            UnitsNetMappers.MapToString(this.AngleUnit)
        );
    }

    protected string UnitToStringMapper(AngleUnit unit)
    {
        return UnitsNetMappers.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(Area unit)
    {
        return new UnitValueDto(unit.As(this.AreaUnit), UnitsNetMappers.MapToString(this.AreaUnit));
    }

    public string UnitToStringMapper(AreaUnit unit)
    {
        return UnitsNetMappers.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(AreaMomentOfInertia unit)
    {
        return new UnitValueDto(
            unit.As(this.AreaMomentOfInertiaUnit),
            UnitsNetMappers.MapToString(this.AreaMomentOfInertiaUnit)
        );
    }

    protected string UnitToStringMapper(AreaMomentOfInertiaUnit unit)
    {
        return UnitsNetMappers.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(Force unit)
    {
        return new UnitValueDto(
            unit.As(this.ForceUnit),
            UnitsNetMappers.MapToString(this.ForceUnit)
        );
    }

    protected string UnitToStringMapper(ForceUnit unit)
    {
        return UnitsNetMappers.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(ForcePerLength unit)
    {
        return new UnitValueDto(
            unit.As(this.ForcePerLengthUnit),
            UnitsNetMappers.MapToString(this.ForcePerLengthUnit)
        );
    }

    protected string UnitToStringMapper(ForcePerLengthUnit unit)
    {
        return UnitsNetMappers.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(Length unit)
    {
        return new UnitValueDto(
            unit.As(this.LengthUnit),
            UnitsNetMappers.MapToString(this.LengthUnit)
        );
    }

    protected string UnitToStringMapper(LengthUnit unit)
    {
        return UnitsNetMappers.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(Pressure unit)
    {
        return new UnitValueDto(
            unit.As(this.PressureUnit),
            UnitsNetMappers.MapToString(this.PressureUnit)
        );
    }

    protected string UnitToStringMapper(PressureUnit unit)
    {
        return UnitsNetMappers.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(Torque unit)
    {
        return new UnitValueDto(
            unit.As(this.TorqueUnit),
            UnitsNetMappers.MapToString(this.TorqueUnit)
        );
    }

    protected string UnitToStringMapper(TorqueUnit unit)
    {
        return UnitsNetMappers.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(Volume unit)
    {
        return new UnitValueDto(
            unit.As(this.VolumeUnit),
            UnitsNetMappers.MapToString(this.VolumeUnit)
        );
    }

    protected string UnitToStringMapper(VolumeUnit unit)
    {
        return UnitsNetMappers.MapToString(unit);
    }
}
