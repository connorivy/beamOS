namespace BeamOs.Contracts.Common;

public static class UnitContractOperators
{
    public static AreaUnitContract ToArea(this LengthUnitContract lengthUnitContract)
    {
        return lengthUnitContract switch
        {
            LengthUnitContract.Centimeter => AreaUnitContract.SquareCentimeter,
            LengthUnitContract.Foot => AreaUnitContract.SquareFoot,
            LengthUnitContract.Inch => AreaUnitContract.SquareInch,
            LengthUnitContract.Meter => AreaUnitContract.SquareMeter,
            LengthUnitContract.Millimeter => AreaUnitContract.SquareMillimeter,
            LengthUnitContract.Undefined or _ => throw new NotImplementedException(),
        };
    }

    public static VolumeUnitContract ToVolume(this LengthUnitContract lengthUnitContract)
    {
        return lengthUnitContract switch
        {
            LengthUnitContract.Centimeter => VolumeUnitContract.CubicCentimeter,
            LengthUnitContract.Foot => VolumeUnitContract.CubicFoot,
            LengthUnitContract.Inch => VolumeUnitContract.CubicInch,
            LengthUnitContract.Meter => VolumeUnitContract.CubicMeter,
            LengthUnitContract.Millimeter => VolumeUnitContract.CubicMillimeter,
            LengthUnitContract.Undefined or _ => throw new NotImplementedException(),
        };
    }

    public static AreaMomentOfInertiaUnitContract ToAreaMomentOfInertia(
        this LengthUnitContract lengthUnitContract
    )
    {
        return lengthUnitContract switch
        {
            LengthUnitContract.Centimeter => AreaMomentOfInertiaUnitContract.CentimeterToTheFourth,
            LengthUnitContract.Foot => AreaMomentOfInertiaUnitContract.FootToTheFourth,
            LengthUnitContract.Inch => AreaMomentOfInertiaUnitContract.InchToTheFourth,
            LengthUnitContract.Meter => AreaMomentOfInertiaUnitContract.MeterToTheFourth,
            LengthUnitContract.Millimeter => AreaMomentOfInertiaUnitContract.MillimeterToTheFourth,
            LengthUnitContract.Undefined or _ => throw new NotImplementedException(),
        };
    }

    public static ForcePerLengthUnitContract DivideBy(
        this ForceUnitContract forceUnitContract,
        LengthUnitContract lengthUnitContract
    )
    {
        return forceUnitContract switch
        {
            ForceUnitContract.Kilonewton
                => lengthUnitContract switch
                {
                    LengthUnitContract.Centimeter
                        => ForcePerLengthUnitContract.KilonewtonPerCentimeter,
                    LengthUnitContract.Meter => ForcePerLengthUnitContract.KilonewtonPerMeter,
                    LengthUnitContract.Millimeter
                        => ForcePerLengthUnitContract.KilonewtonPerMillimeter,
                },
            ForceUnitContract.KilopoundForce
                => lengthUnitContract switch
                {
                    LengthUnitContract.Foot => ForcePerLengthUnitContract.KilopoundForcePerFoot,
                    LengthUnitContract.Inch => ForcePerLengthUnitContract.KilopoundForcePerInch,
                },
            ForceUnitContract.Newton
                => lengthUnitContract switch
                {
                    LengthUnitContract.Centimeter => ForcePerLengthUnitContract.NewtonPerCentimeter,
                    LengthUnitContract.Meter => ForcePerLengthUnitContract.NewtonPerMeter,
                    LengthUnitContract.Millimeter => ForcePerLengthUnitContract.NewtonPerMillimeter,
                },
            ForceUnitContract.PoundForce
                => lengthUnitContract switch
                {
                    LengthUnitContract.Foot => ForcePerLengthUnitContract.PoundForcePerFoot,
                    LengthUnitContract.Inch => ForcePerLengthUnitContract.PoundForcePerInch,
                },
            ForceUnitContract.Undefined or _ => throw new NotImplementedException(),
        };
    }

    public static TorqueUnitContract MultiplyBy(
        this ForceUnitContract forceUnitContract,
        LengthUnitContract lengthUnitContract
    )
    {
        return forceUnitContract switch
        {
            ForceUnitContract.Kilonewton
                => lengthUnitContract switch
                {
                    LengthUnitContract.Centimeter => TorqueUnitContract.KilonewtonCentimeter,
                    LengthUnitContract.Meter => TorqueUnitContract.KilonewtonMeter,
                    LengthUnitContract.Millimeter => TorqueUnitContract.KilonewtonMillimeter,
                },
            ForceUnitContract.KilopoundForce
                => lengthUnitContract switch
                {
                    LengthUnitContract.Foot => TorqueUnitContract.KilopoundForceFoot,
                    LengthUnitContract.Inch => TorqueUnitContract.KilopoundForceInch,
                },
            ForceUnitContract.Newton
                => lengthUnitContract switch
                {
                    LengthUnitContract.Centimeter => TorqueUnitContract.NewtonCentimeter,
                    LengthUnitContract.Meter => TorqueUnitContract.NewtonMeter,
                    LengthUnitContract.Millimeter => TorqueUnitContract.NewtonMillimeter,
                },
            ForceUnitContract.PoundForce
                => lengthUnitContract switch
                {
                    LengthUnitContract.Foot => TorqueUnitContract.PoundForceFoot,
                    LengthUnitContract.Inch => TorqueUnitContract.PoundForceInch,
                },
            ForceUnitContract.Undefined or _ => throw new NotImplementedException(),
        };
    }

    public static PressureUnitContract GetPressure(
        this ForceUnitContract forceUnitContract,
        LengthUnitContract lengthUnitContract
    )
    {
        return forceUnitContract switch
        {
            ForceUnitContract.Kilonewton
                => lengthUnitContract switch
                {
                    LengthUnitContract.Centimeter
                        => PressureUnitContract.KilonewtonPerSquareCentimeter,
                    LengthUnitContract.Meter => PressureUnitContract.KilonewtonPerSquareMeter,
                    LengthUnitContract.Millimeter
                        => PressureUnitContract.KilonewtonPerSquareMillimeter,
                },
            ForceUnitContract.KilopoundForce
                => lengthUnitContract switch
                {
                    LengthUnitContract.Foot => PressureUnitContract.KilopoundForcePerSquareFoot,
                    LengthUnitContract.Inch => PressureUnitContract.KilopoundForcePerSquareInch,
                },
            ForceUnitContract.Newton
                => lengthUnitContract switch
                {
                    LengthUnitContract.Centimeter => PressureUnitContract.NewtonPerSquareCentimeter,
                    LengthUnitContract.Meter => PressureUnitContract.NewtonPerSquareMeter,
                    LengthUnitContract.Millimeter => PressureUnitContract.NewtonPerSquareMillimeter,
                },
            ForceUnitContract.PoundForce
                => lengthUnitContract switch
                {
                    LengthUnitContract.Foot => PressureUnitContract.PoundForcePerSquareFoot,
                    LengthUnitContract.Inch => PressureUnitContract.PoundForcePerSquareInch,
                },
            ForceUnitContract.Undefined or _ => throw new NotImplementedException(),
        };
    }
}
