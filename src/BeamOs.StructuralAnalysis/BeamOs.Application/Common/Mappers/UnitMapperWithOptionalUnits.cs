using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
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
                UnitsNetMappers.MapToString(unitType.Value)
            );
        }
        return UnitsNetMappers.MapToContract(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(Area unit)
    {
        var unitType = this.AreaUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                UnitsNetMappers.MapToString(unitType.Value)
            );
        }
        return UnitsNetMappers.MapToContract(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(AreaMomentOfInertia unit)
    {
        var unitType = this.AreaMomentOfInertiaUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                UnitsNetMappers.MapToString(unitType.Value)
            );
        }
        return UnitsNetMappers.MapToContract(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(Force unit)
    {
        var unitType = this.ForceUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                UnitsNetMappers.MapToString(unitType.Value)
            );
        }
        return UnitsNetMappers.MapToContract(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(ForcePerLength unit)
    {
        var unitType = this.ForcePerLengthUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                UnitsNetMappers.MapToString(unitType.Value)
            );
        }
        return UnitsNetMappers.MapToContract(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(Length unit)
    {
        var unitType = this.LengthUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                UnitsNetMappers.MapToString(unitType.Value)
            );
        }
        return UnitsNetMappers.MapToContract(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(Pressure unit)
    {
        var unitType = this.PressureUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                UnitsNetMappers.MapToString(unitType.Value)
            );
        }
        return UnitsNetMappers.MapToContract(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(Torque unit)
    {
        var unitType = this.TorqueUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                UnitsNetMappers.MapToString(unitType.Value)
            );
        }
        return UnitsNetMappers.MapToContract(unit);
    }

    public UnitValueDto UnitToUnitValueDtoMapper(Volume unit)
    {
        var unitType = this.VolumeUnit;
        if (unitType != null)
        {
            return new UnitValueDto(
                unit.As(unitType.Value),
                UnitsNetMappers.MapToString(unitType.Value)
            );
        }
        return UnitsNetMappers.MapToContract(unit);
    }
}
