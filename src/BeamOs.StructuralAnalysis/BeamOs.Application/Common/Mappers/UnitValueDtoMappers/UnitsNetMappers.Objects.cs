using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Application.Common.Mappers.UnitValueDtoMappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class UnitsNetMappers
{
    public static partial Angle MapToAngle(this UnitValueDto unit);

    public static partial UnitValueDto MapToContract(this Angle unit);

    public static partial Area MapToArea(this UnitValueDto unit);

    public static partial UnitValueDto MapToContract(this Area unit);

    public static partial AreaMomentOfInertia MapToAreaMomentOfInertia(this UnitValueDto unit);

    public static partial UnitValueDto MapToContract(this AreaMomentOfInertia unit);

    public static partial Force MapToForce(this UnitValueDto unit);

    public static partial UnitValueDto MapToContract(this Force unit);

    public static partial ForcePerLength MapToForcePerLength(this UnitValueDto unit);

    public static partial UnitValueDto MapToContract(this ForcePerLength unit);

    public static partial Length MapToLength(this UnitValueDto unit);

    public static partial UnitValueDto MapToContract(this Length unit);

    public static UnitValueDto MapToContract(this Length value, LengthUnit unit)
    {
        return new(value.As(unit), MapToString(unit));
    }

    public static partial Pressure MapToPressure(this UnitValueDto unit);

    public static partial UnitValueDto MapToContract(this Pressure unit);

    public static partial Torque MapToTorque(this UnitValueDto unit);

    public static partial UnitValueDto MapToContract(this Torque unit);

    public static partial Volume MapToVolume(this UnitValueDto unit);

    public static partial UnitValueDto MapToContract(this Volume unit);
}
