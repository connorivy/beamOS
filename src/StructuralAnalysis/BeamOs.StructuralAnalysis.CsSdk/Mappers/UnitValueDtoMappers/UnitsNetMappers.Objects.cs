using BeamOs.StructuralAnalysis.Contracts.Common;
using Riok.Mapperly.Abstractions;
using UnitsNet;

namespace BeamOs.Application.Common.Mappers.UnitValueDtoMappers;

[Mapper(PreferParameterlessConstructors = false, EnumMappingStrategy = EnumMappingStrategy.ByName)]
public static partial class UnitsNetMappers
{
    public static partial Angle MapToAngle(this AngleContract unit);

    public static partial AngleContract MapToContract(this Angle unit);

    public static partial Area MapToArea(this AreaContract unit);

    public static partial AreaContract MapToContract(this Area unit);

    public static partial AreaMomentOfInertia MapToAreaMomentOfInertia(
        this AreaMomentOfInertiaContract unit
    );

    public static partial AreaMomentOfInertiaContract MapToContract(this AreaMomentOfInertia unit);

    public static partial Force MapToForce(this ForceContract unit);

    public static partial ForceContract MapToContract(this Force unit);

    public static partial ForcePerLength MapToForcePerLength(this ForcePerLengthContract unit);

    public static partial ForcePerLengthContract MapToContract(this ForcePerLength unit);

    public static partial Length MapToLength(this LengthContract unit);

    public static partial LengthContract MapToContract(this Length unit);

    public static partial Pressure MapToPressure(this PressureContract unit);

    public static partial PressureContract MapToContract(this Pressure unit);

    public static partial Torque MapToTorque(this TorqueContract unit);

    public static partial TorqueContract MapToContract(this Torque unit);

    public static partial Volume MapToVolume(this VolumeContract unit);

    public static partial VolumeContract MapToContract(this Volume unit);
}
