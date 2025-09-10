namespace BeamOs.StructuralAnalysis.Contracts.Common;

public static class UnitOperators
{
    public static AreaUnit ToArea(this LengthUnit lengthUnit)
    {
        return lengthUnit switch
        {
            LengthUnit.Centimeter => AreaUnit.SquareCentimeter,
            LengthUnit.Foot => AreaUnit.SquareFoot,
            LengthUnit.Inch => AreaUnit.SquareInch,
            LengthUnit.Meter => AreaUnit.SquareMeter,
            LengthUnit.Millimeter => AreaUnit.SquareMillimeter,
            LengthUnit.Undefined or _ => throw new NotImplementedException(),
        };
    }

    public static VolumeUnit ToVolume(this LengthUnit lengthUnit)
    {
        return lengthUnit switch
        {
            LengthUnit.Centimeter => VolumeUnit.CubicCentimeter,
            LengthUnit.Foot => VolumeUnit.CubicFoot,
            LengthUnit.Inch => VolumeUnit.CubicInch,
            LengthUnit.Meter => VolumeUnit.CubicMeter,
            LengthUnit.Millimeter => VolumeUnit.CubicMillimeter,
            LengthUnit.Undefined or _ => throw new NotImplementedException(),
        };
    }

    public static AreaMomentOfInertiaUnit ToAreaMomentOfInertia(this LengthUnit lengthUnit)
    {
        return lengthUnit switch
        {
            LengthUnit.Centimeter => AreaMomentOfInertiaUnit.CentimeterToTheFourth,
            LengthUnit.Foot => AreaMomentOfInertiaUnit.FootToTheFourth,
            LengthUnit.Inch => AreaMomentOfInertiaUnit.InchToTheFourth,
            LengthUnit.Meter => AreaMomentOfInertiaUnit.MeterToTheFourth,
            LengthUnit.Millimeter => AreaMomentOfInertiaUnit.MillimeterToTheFourth,
            LengthUnit.Undefined or _ => throw new NotImplementedException(),
        };
    }

    public static ForcePerLengthUnit DivideBy(this ForceUnit forceUnit, LengthUnit lengthUnit)
    {
        return forceUnit switch
        {
            ForceUnit.Kilonewton
                => lengthUnit switch
                {
                    LengthUnit.Centimeter => ForcePerLengthUnit.KilonewtonPerCentimeter,
                    LengthUnit.Meter => ForcePerLengthUnit.KilonewtonPerMeter,
                    LengthUnit.Millimeter => ForcePerLengthUnit.KilonewtonPerMillimeter,
                },
            ForceUnit.KilopoundForce
                => lengthUnit switch
                {
                    LengthUnit.Foot => ForcePerLengthUnit.KilopoundForcePerFoot,
                    LengthUnit.Inch => ForcePerLengthUnit.KilopoundForcePerInch,
                },
            ForceUnit.Newton
                => lengthUnit switch
                {
                    LengthUnit.Centimeter => ForcePerLengthUnit.NewtonPerCentimeter,
                    LengthUnit.Meter => ForcePerLengthUnit.NewtonPerMeter,
                    LengthUnit.Millimeter => ForcePerLengthUnit.NewtonPerMillimeter,
                },
            ForceUnit.PoundForce
                => lengthUnit switch
                {
                    LengthUnit.Foot => ForcePerLengthUnit.PoundForcePerFoot,
                    LengthUnit.Inch => ForcePerLengthUnit.PoundForcePerInch,
                },
            ForceUnit.Undefined or _ => throw new NotImplementedException(),
        };
    }

    public static TorqueUnit MultiplyBy(this ForceUnit forceUnit, LengthUnit lengthUnit)
    {
        return forceUnit switch
        {
            ForceUnit.Kilonewton
                => lengthUnit switch
                {
                    LengthUnit.Centimeter => TorqueUnit.KilonewtonCentimeter,
                    LengthUnit.Meter => TorqueUnit.KilonewtonMeter,
                    LengthUnit.Millimeter => TorqueUnit.KilonewtonMillimeter,
                },
            ForceUnit.KilopoundForce
                => lengthUnit switch
                {
                    LengthUnit.Foot => TorqueUnit.KilopoundForceFoot,
                    LengthUnit.Inch => TorqueUnit.KilopoundForceInch,
                },
            ForceUnit.Newton
                => lengthUnit switch
                {
                    LengthUnit.Centimeter => TorqueUnit.NewtonCentimeter,
                    LengthUnit.Meter => TorqueUnit.NewtonMeter,
                    LengthUnit.Millimeter => TorqueUnit.NewtonMillimeter,
                },
            ForceUnit.PoundForce
                => lengthUnit switch
                {
                    LengthUnit.Foot => TorqueUnit.PoundForceFoot,
                    LengthUnit.Inch => TorqueUnit.PoundForceInch,
                },
            ForceUnit.Undefined or _ => throw new NotImplementedException(),
        };
    }

    public static PressureUnit GetPressure(this ForceUnit forceUnit, LengthUnit lengthUnit)
    {
        return forceUnit switch
        {
            ForceUnit.Kilonewton
                => lengthUnit switch
                {
                    LengthUnit.Centimeter => PressureUnit.KilonewtonPerSquareCentimeter,
                    LengthUnit.Meter => PressureUnit.KilonewtonPerSquareMeter,
                    LengthUnit.Millimeter => PressureUnit.KilonewtonPerSquareMillimeter,
                },
            ForceUnit.KilopoundForce
                => lengthUnit switch
                {
                    LengthUnit.Foot => PressureUnit.KilopoundForcePerSquareFoot,
                    LengthUnit.Inch => PressureUnit.KilopoundForcePerSquareInch,
                },
            ForceUnit.Newton
                => lengthUnit switch
                {
                    LengthUnit.Centimeter => PressureUnit.NewtonPerSquareCentimeter,
                    LengthUnit.Meter => PressureUnit.NewtonPerSquareMeter,
                    LengthUnit.Millimeter => PressureUnit.NewtonPerSquareMillimeter,
                },
            ForceUnit.PoundForce
                => lengthUnit switch
                {
                    LengthUnit.Foot => PressureUnit.PoundForcePerSquareFoot,
                    LengthUnit.Inch => PressureUnit.PoundForcePerSquareInch,
                },
            ForceUnit.Undefined or _ => throw new NotImplementedException(),
        };
    }
}
