using BeamOs.Api.Common.Mappers;
using BeamOs.Api.Common.Mappers.UnitValueDtoMappers;
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
    protected AbstractMapperProvidedUnits(UnitSettings unitSettings)
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
            AngleUnitToStringMapper.MapToString(this.AngleUnit)
        );
    }

    protected string UnitToStringMapper(AngleUnit unit)
    {
        return AngleUnitToStringMapper.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(Area unit)
    {
        return new UnitValueDto(
            unit.As(this.AreaUnit),
            AreaUnitToStringMapper.MapToString(this.AreaUnit)
        );
    }

    public string UnitToStringMapper(AreaUnit unit)
    {
        return AreaUnitToStringMapper.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(AreaMomentOfInertia unit)
    {
        return new UnitValueDto(
            unit.As(this.AreaMomentOfInertiaUnit),
            AreaMomentOfInertiaUnitToStringMapper.MapToString(this.AreaMomentOfInertiaUnit)
        );
    }

    protected string UnitToStringMapper(AreaMomentOfInertiaUnit unit)
    {
        return AreaMomentOfInertiaUnitToStringMapper.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(Force unit)
    {
        return new UnitValueDto(
            unit.As(this.ForceUnit),
            ForceUnitToStringMapper.MapToString(this.ForceUnit)
        );
    }

    protected string UnitToStringMapper(ForceUnit unit)
    {
        return ForceUnitToStringMapper.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(ForcePerLength unit)
    {
        return new UnitValueDto(
            unit.As(this.ForcePerLengthUnit),
            ForcePerLengthUnitToStringMapper.MapToString(this.ForcePerLengthUnit)
        );
    }

    protected string UnitToStringMapper(ForcePerLengthUnit unit)
    {
        return ForcePerLengthUnitToStringMapper.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(Length unit)
    {
        return new UnitValueDto(
            unit.As(this.LengthUnit),
            LengthUnitToStringMapper.MapToString(this.LengthUnit)
        );
    }

    protected string UnitToStringMapper(LengthUnit unit)
    {
        return LengthUnitToStringMapper.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(Pressure unit)
    {
        return new UnitValueDto(
            unit.As(this.PressureUnit),
            PressureUnitToStringMapper.MapToString(this.PressureUnit)
        );
    }

    protected string UnitToStringMapper(PressureUnit unit)
    {
        return PressureUnitToStringMapper.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(Torque unit)
    {
        return new UnitValueDto(
            unit.As(this.TorqueUnit),
            TorqueUnitToStringMapper.MapToString(this.TorqueUnit)
        );
    }

    protected string UnitToStringMapper(TorqueUnit unit)
    {
        return TorqueUnitToStringMapper.MapToString(unit);
    }

    protected UnitValueDto UnitToUnitValueDtoMapper(Volume unit)
    {
        return new UnitValueDto(
            unit.As(this.VolumeUnit),
            VolumeUnitToStringMapper.MapToString(this.VolumeUnit)
        );
    }

    protected string UnitToStringMapper(VolumeUnit unit)
    {
        return VolumeUnitToStringMapper.MapToString(unit);
    }
}

public interface IMapperWithUnits<TFrom, TTo, TUnit1>
{
    public TTo Map(TFrom source, TUnit1 unit1);
}
