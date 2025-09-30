using Riok.Mapperly.Abstractions;

[Mapper(PreferParameterlessConstructors = false, EnumMappingStrategy = EnumMappingStrategy.ByName)]
internal static partial class UnitContractExtensions
{
    public static double As(this AngleContract value, AngleUnitContract targetUnit)
    {
        return value.ToUnitsNet().As(targetUnit.ToUnitsNet());
    }

    public static partial Angle ToUnitsNet(this AngleContract value);

    public static partial AngleUnit ToUnitsNet(this AngleUnitContract valueUnit);

    public static double As(this AreaContract value, AreaUnitContract targetUnit)
    {
        return value.ToUnitsNet().As(targetUnit.ToUnitsNet());
    }

    public static partial Area ToUnitsNet(this AreaContract value);

    public static partial AreaUnit ToUnitsNet(this AreaUnitContract valueUnit);

    public static double As(
        this AreaMomentOfInertiaContract value,
        AreaMomentOfInertiaUnitContract targetUnit
    )
    {
        return value.ToUnitsNet().As(targetUnit.ToUnitsNet());
    }

    public static partial AreaMomentOfInertia ToUnitsNet(this AreaMomentOfInertiaContract value);

    public static partial AreaMomentOfInertiaUnit ToUnitsNet(
        this AreaMomentOfInertiaUnitContract valueUnit
    );

    public static double As(this ForceContract value, ForceUnitContract targetUnit)
    {
        return value.ToUnitsNet().As(targetUnit.ToUnitsNet());
    }

    public static partial Force ToUnitsNet(this ForceContract value);

    public static partial ForceUnit ToUnitsNet(this ForceUnitContract valueUnit);

    public static double As(this LengthContract value, LengthUnitContract targetUnit)
    {
        return value.ToUnitsNet().As(targetUnit.ToUnitsNet());
    }

    public static partial Length ToUnitsNet(this LengthContract value);

    public static partial LengthUnit ToUnitsNet(this LengthUnitContract valueUnit);

    public static double As(this PressureContract value, PressureUnitContract targetUnit)
    {
        return value.ToUnitsNet().As(targetUnit.ToUnitsNet());
    }

    public static partial Pressure ToUnitsNet(this PressureContract value);

    public static partial PressureUnit ToUnitsNet(this PressureUnitContract valueUnit);

    public static double As(this TorqueContract value, TorqueUnitContract targetUnit)
    {
        return value.ToUnitsNet().As(targetUnit.ToUnitsNet());
    }

    public static partial Torque ToUnitsNet(this TorqueContract value);

    public static partial TorqueUnit ToUnitsNet(this TorqueUnitContract valueUnit);

    public static double As(this VolumeContract value, VolumeUnitContract targetUnit)
    {
        return value.ToUnitsNet().As(targetUnit.ToUnitsNet());
    }

    public static partial Volume ToUnitsNet(this VolumeContract value);

    public static partial VolumeUnit ToUnitsNet(this VolumeUnitContract valueUnit);
}
