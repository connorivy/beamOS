using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Application.Common.Mappers.UnitValueDtoMappers;

[Mapper(PreferParameterlessConstructors = false, EnumMappingStrategy = EnumMappingStrategy.ByName)]
public static partial class UnitsNetMappers
{
    public static partial Angle MapToAngle(this UnitValueDto unit);

    public static partial Angle MapToAngle(this AngleContract unit);

    public static partial UnitValueDto MapToContract(this Angle unit);

    public static partial AngleContract MapToContract2(this Angle unit);

    public static partial Area MapToArea(this UnitValueDto unit);

    public static partial Area MapToArea(this AreaContract unit);

    public static partial UnitValueDto MapToContract(this Area unit);

    public static partial AreaContract MapToContract2(this Area unit);

    public static partial AreaMomentOfInertia MapToAreaMomentOfInertia(this UnitValueDto unit);

    public static partial AreaMomentOfInertia MapToAreaMomentOfInertia(
        this AreaMomentOfInertiaContract unit
    );

    public static partial UnitValueDto MapToContract(this AreaMomentOfInertia unit);

    public static partial AreaMomentOfInertiaContract MapToContract2(this AreaMomentOfInertia unit);

    public static partial Force MapToForce(this UnitValueDto unit);

    public static partial Force MapToForce(this ForceContract unit);

    public static partial UnitValueDto MapToContract(this Force unit);

    public static partial ForceContract MapToContract2(this Force unit);

    public static partial ForcePerLength MapToForcePerLength(this UnitValueDto unit);

    public static partial ForcePerLength MapToForcePerLength2(this ForcePerLengthContract unit);

    public static partial UnitValueDto MapToContract(this ForcePerLength unit);

    public static partial ForcePerLengthContract MapToContract2(this ForcePerLength unit);

    public static partial Length MapToLength(this UnitValueDto unit);

    public static partial Length MapToLength2(this LengthContract unit);

    public static partial UnitValueDto MapToContract(this Length unit);

    public static partial LengthContract MapToContract2(this Length unit);

    public static UnitValueDto MapToContract(this Length value, LengthUnit unit)
    {
        return new(value.As(unit), MapToString(unit));
    }

    public static partial Pressure MapToPressure(this UnitValueDto unit);

    public static partial Pressure MapToPressure2(this PressureContract unit);

    public static partial UnitValueDto MapToContract(this Pressure unit);

    public static partial PressureContract MapToContract2(this Pressure unit);

    public static partial Torque MapToTorque(this UnitValueDto unit);

    public static partial Torque MapToTorque2(this TorqueContract unit);

    public static partial UnitValueDto MapToContract(this Torque unit);

    public static partial TorqueContract MapToContract2(this Torque unit);

    public static partial Volume MapToVolume(this UnitValueDto unit);

    public static partial Volume MapToVolume2(this VolumeContract unit);

    public static partial UnitValueDto MapToContract(this Volume unit);

    public static partial VolumeContract MapToContract2(this Volume unit);
}
