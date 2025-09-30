using Riok.Mapperly.Abstractions;

namespace BeamOs.Tests.Common.Mappers.UnitValueDtoMappers;

[Mapper(PreferParameterlessConstructors = false, EnumMappingStrategy = EnumMappingStrategy.ByName)]
public static partial class UnitsNetMappers
{
    public static partial Angle MapToAngle(this AngleContract unit);

    public static double As(this AngleContract value, AngleUnitContract unitContract) =>
        value.MapToAngle().As(unitContract.MapToAngleUnit());

    public static partial AngleContract MapToContract(this Angle unit);

    public static partial Area MapToArea(this AreaContract unit);

    public static double As(this AreaContract unit, AreaUnitContract unitContract) =>
        unit.MapToArea().As(unitContract.MapToAreaUnit());

    public static partial AreaContract MapToContract(this Area unit);

    public static partial AreaMomentOfInertia MapToAreaMomentOfInertia(
        this AreaMomentOfInertiaContract unit
    );

    public static double As(
        this AreaMomentOfInertiaContract unit,
        AreaMomentOfInertiaUnitContract unitContract
    ) => unit.MapToAreaMomentOfInertia().As(unitContract.MapToAreaMomentOfInertiaUnit());

    public static partial AreaMomentOfInertiaContract MapToContract(this AreaMomentOfInertia unit);

    public static partial Force MapToForce(this ForceContract unit);

    public static double As(this ForceContract unit, ForceUnitContract unitContract) =>
        unit.MapToForce().As(unitContract.MapToForceUnit());

    public static partial ForceContract MapToContract(this Force unit);

    public static partial ForcePerLength MapToForcePerLength(this ForcePerLengthContract unit);

    public static partial ForcePerLengthContract MapToContract(this ForcePerLength unit);

    public static partial Length MapToLength(this LengthContract unit);

    public static partial LengthContract MapToContract(this Length unit);

    public static partial Pressure MapToPressure(this PressureContract unit);

    public static double As(this PressureContract unit, PressureUnitContract unitContract) =>
        unit.MapToPressure().As(unitContract.MapToPressureUnit());

    public static partial PressureContract MapToContract(this Pressure unit);

    public static partial Ratio MapToRatio(this RatioContract unit);

    public static double As(this RatioContract unit, RatioUnitContract unitContract) =>
        unit.MapToRatio().As(unitContract.MapToRatioUnit());

    public static partial RatioContract MapToContract(this Ratio unit);

    public static partial Torque MapToTorque(this TorqueContract unit);

    public static double As(this TorqueContract unit, TorqueUnitContract unitContract) =>
        unit.MapToTorque().As(unitContract.MapToTorqueUnit());

    public static partial TorqueContract MapToContract(this Torque unit);

    public static partial Volume MapToVolume(this VolumeContract unit);

    public static partial VolumeContract MapToContract(this Volume unit);
}
