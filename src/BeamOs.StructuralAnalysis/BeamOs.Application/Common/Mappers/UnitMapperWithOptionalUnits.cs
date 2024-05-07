using BeamOs.Api.Common.Mappers;
using BeamOs.Api.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.Common;
using BeamOs.Domain.Common.ValueObjects;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Application.Common.Mappers;

public class UnitMapperWithOptionalUnits()
{
    protected LengthUnit? LengthUnit { get; init; }
    protected AreaUnit? AreaUnit { get; init; }
    protected VolumeUnit? VolumeUnit { get; init; }
    protected ForceUnit? ForceUnit { get; init; }
    protected ForcePerLengthUnit? ForcePerLengthUnit { get; init; }
    protected TorqueUnit? TorqueUnit { get; init; }
    protected AngleUnit? AngleUnit { get; init; }
    protected PressureUnit? PressureUnit { get; init; }
    protected AreaMomentOfInertiaUnit? AreaMomentOfInertiaUnit { get; init; }

    public UnitMapperWithOptionalUnits(UnitSettings unitSettings)
        : this()
    {
        this.LengthUnit = unitSettings.LengthUnit;
        this.AngleUnit = unitSettings.AngleUnit;
        this.VolumeUnit = unitSettings.VolumeUnit;
        this.ForceUnit = unitSettings.ForceUnit;
        this.ForcePerLengthUnit = unitSettings.ForcePerLengthUnit;
        this.TorqueUnit = unitSettings.TorqueUnit;
        this.AngleUnit = UnitsNet.Units.AngleUnit.Radian;
        this.PressureUnit = unitSettings.PressureUnit;
        this.AreaMomentOfInertiaUnit = unitSettings.AreaMomentOfInertiaUnit;
    }

    public UnitValueDto UnitToUnitValueDtoMapper(Angle unit)
    {
        var unitType = this.AngleUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                AngleUnitToStringMapper.MapToString(unitType.Value)
            );
        }
        return new UnitValueDto(unit.Value, AngleUnitToStringMapper.MapToString(unit.Unit));
    }

    public static string UnitToStringMapper(AngleUnit unit)
    {
        return AngleUnitToStringMapper.MapToString(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(Area unit)
    {
        var unitType = this.AreaUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                AreaUnitToStringMapper.MapToString(unitType.Value)
            );
        }
        return new UnitValueDto(unit.Value, AreaUnitToStringMapper.MapToString(unit.Unit));
    }

    public static string UnitToStringMapper(AreaUnit unit)
    {
        return AreaUnitToStringMapper.MapToString(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(AreaMomentOfInertia unit)
    {
        var unitType = this.AreaMomentOfInertiaUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                AreaMomentOfInertiaUnitToStringMapper.MapToString(unitType.Value)
            );
        }
        return new UnitValueDto(
            unit.Value,
            AreaMomentOfInertiaUnitToStringMapper.MapToString(unit.Unit)
        );
    }

    public static string UnitToStringMapper(AreaMomentOfInertiaUnit unit)
    {
        return AreaMomentOfInertiaUnitToStringMapper.MapToString(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(Force unit)
    {
        var unitType = this.ForceUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                ForceUnitToStringMapper.MapToString(unitType.Value)
            );
        }
        return new UnitValueDto(unit.Value, ForceUnitToStringMapper.MapToString(unit.Unit));
    }

    public static string UnitToStringMapper(ForceUnit unit)
    {
        return ForceUnitToStringMapper.MapToString(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(ForcePerLength unit)
    {
        var unitType = this.ForcePerLengthUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                ForcePerLengthUnitToStringMapper.MapToString(unitType.Value)
            );
        }
        return new UnitValueDto(
            unit.Value,
            ForcePerLengthUnitToStringMapper.MapToString(unit.Unit)
        );
    }

    public static string UnitToStringMapper(ForcePerLengthUnit unit)
    {
        return ForcePerLengthUnitToStringMapper.MapToString(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(Length unit)
    {
        var unitType = this.LengthUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                LengthUnitToStringMapper.MapToString(unitType.Value)
            );
        }
        return new UnitValueDto(unit.Value, LengthUnitToStringMapper.MapToString(unit.Unit));
    }

    public static string UnitToStringMapper(LengthUnit unit)
    {
        return LengthUnitToStringMapper.MapToString(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(Pressure unit)
    {
        var unitType = this.PressureUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                PressureUnitToStringMapper.MapToString(unitType.Value)
            );
        }
        return new UnitValueDto(unit.Value, PressureUnitToStringMapper.MapToString(unit.Unit));
    }

    public static string UnitToStringMapper(PressureUnit unit)
    {
        return PressureUnitToStringMapper.MapToString(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(Torque unit)
    {
        var unitType = this.TorqueUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                TorqueUnitToStringMapper.MapToString(unitType.Value)
            );
        }
        return new UnitValueDto(unit.Value, TorqueUnitToStringMapper.MapToString(unit.Unit));
    }

    public static string UnitToStringMapper(TorqueUnit unit)
    {
        return TorqueUnitToStringMapper.MapToString(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(Volume unit)
    {
        var unitType = this.VolumeUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                VolumeUnitToStringMapper.MapToString(unitType.Value)
            );
        }
        return new UnitValueDto(unit.Value, VolumeUnitToStringMapper.MapToString(unit.Unit));
    }

    public static string UnitToStringMapper(VolumeUnit unit)
    {
        return VolumeUnitToStringMapper.MapToString(unit);
    }
}
