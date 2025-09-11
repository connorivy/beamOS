using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Application.Common;

internal static class UnitOperators
{
    public static AreaUnit ToArea(this LengthUnit lengthUnit)
    {
        return lengthUnit switch
        {
            LengthUnit.Angstrom => throw new NotImplementedException(),
            LengthUnit.AstronomicalUnit => throw new NotImplementedException(),
            LengthUnit.Centimeter => AreaUnit.SquareCentimeter,
            LengthUnit.Chain => throw new NotImplementedException(),
            LengthUnit.DataMile => throw new NotImplementedException(),
            LengthUnit.Decameter => throw new NotImplementedException(),
            LengthUnit.Decimeter => throw new NotImplementedException(),
            LengthUnit.DtpPica => throw new NotImplementedException(),
            LengthUnit.DtpPoint => throw new NotImplementedException(),
            LengthUnit.Fathom => throw new NotImplementedException(),
            LengthUnit.Femtometer => throw new NotImplementedException(),
            LengthUnit.Foot => AreaUnit.SquareFoot,
            LengthUnit.Gigameter => throw new NotImplementedException(),
            LengthUnit.Hand => throw new NotImplementedException(),
            LengthUnit.Hectometer => throw new NotImplementedException(),
            LengthUnit.Inch => AreaUnit.SquareInch,
            LengthUnit.Kilofoot => throw new NotImplementedException(),
            LengthUnit.KilolightYear => throw new NotImplementedException(),
            LengthUnit.Kilometer => throw new NotImplementedException(),
            LengthUnit.Kiloparsec => throw new NotImplementedException(),
            LengthUnit.LightYear => throw new NotImplementedException(),
            LengthUnit.MegalightYear => throw new NotImplementedException(),
            LengthUnit.Megameter => throw new NotImplementedException(),
            LengthUnit.Megaparsec => throw new NotImplementedException(),
            LengthUnit.Meter => AreaUnit.SquareMeter,
            LengthUnit.Microinch => throw new NotImplementedException(),
            LengthUnit.Micrometer => throw new NotImplementedException(),
            LengthUnit.Mil => throw new NotImplementedException(),
            LengthUnit.Mile => throw new NotImplementedException(),
            LengthUnit.Millimeter => AreaUnit.SquareMillimeter,
            LengthUnit.Nanometer => throw new NotImplementedException(),
            LengthUnit.NauticalMile => throw new NotImplementedException(),
            LengthUnit.Parsec => throw new NotImplementedException(),
            LengthUnit.Picometer => throw new NotImplementedException(),
            LengthUnit.PrinterPica => throw new NotImplementedException(),
            LengthUnit.PrinterPoint => throw new NotImplementedException(),
            LengthUnit.Shackle => throw new NotImplementedException(),
            LengthUnit.SolarRadius => throw new NotImplementedException(),
            LengthUnit.Twip => throw new NotImplementedException(),
            LengthUnit.UsSurveyFoot => throw new NotImplementedException(),
            LengthUnit.Yard => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };
    }

    public static VolumeUnit ToVolume(this LengthUnit lengthUnit)
    {
        return lengthUnit switch
        {
            LengthUnit.Angstrom => throw new NotImplementedException(),
            LengthUnit.AstronomicalUnit => throw new NotImplementedException(),
            LengthUnit.Centimeter => VolumeUnit.CubicCentimeter,
            LengthUnit.Chain => throw new NotImplementedException(),
            LengthUnit.DataMile => throw new NotImplementedException(),
            LengthUnit.Decameter => throw new NotImplementedException(),
            LengthUnit.Decimeter => throw new NotImplementedException(),
            LengthUnit.DtpPica => throw new NotImplementedException(),
            LengthUnit.DtpPoint => throw new NotImplementedException(),
            LengthUnit.Fathom => throw new NotImplementedException(),
            LengthUnit.Femtometer => throw new NotImplementedException(),
            LengthUnit.Foot => VolumeUnit.CubicFoot,
            LengthUnit.Gigameter => throw new NotImplementedException(),
            LengthUnit.Hand => throw new NotImplementedException(),
            LengthUnit.Hectometer => throw new NotImplementedException(),
            LengthUnit.Inch => VolumeUnit.CubicInch,
            LengthUnit.Kilofoot => throw new NotImplementedException(),
            LengthUnit.KilolightYear => throw new NotImplementedException(),
            LengthUnit.Kilometer => throw new NotImplementedException(),
            LengthUnit.Kiloparsec => throw new NotImplementedException(),
            LengthUnit.LightYear => throw new NotImplementedException(),
            LengthUnit.MegalightYear => throw new NotImplementedException(),
            LengthUnit.Megameter => throw new NotImplementedException(),
            LengthUnit.Megaparsec => throw new NotImplementedException(),
            LengthUnit.Meter => VolumeUnit.CubicMeter,
            LengthUnit.Microinch => throw new NotImplementedException(),
            LengthUnit.Micrometer => throw new NotImplementedException(),
            LengthUnit.Mil => throw new NotImplementedException(),
            LengthUnit.Mile => throw new NotImplementedException(),
            LengthUnit.Millimeter => VolumeUnit.CubicMillimeter,
            LengthUnit.Nanometer => throw new NotImplementedException(),
            LengthUnit.NauticalMile => throw new NotImplementedException(),
            LengthUnit.Parsec => throw new NotImplementedException(),
            LengthUnit.Picometer => throw new NotImplementedException(),
            LengthUnit.PrinterPica => throw new NotImplementedException(),
            LengthUnit.PrinterPoint => throw new NotImplementedException(),
            LengthUnit.Shackle => throw new NotImplementedException(),
            LengthUnit.SolarRadius => throw new NotImplementedException(),
            LengthUnit.Twip => throw new NotImplementedException(),
            LengthUnit.UsSurveyFoot => throw new NotImplementedException(),
            LengthUnit.Yard => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };
    }

    public static AreaMomentOfInertiaUnit ToAreaMomentOfInertiaUnit(this LengthUnit lengthUnit)
    {
        return lengthUnit switch
        {
            LengthUnit.Angstrom => throw new NotImplementedException(),
            LengthUnit.AstronomicalUnit => throw new NotImplementedException(),
            LengthUnit.Centimeter => AreaMomentOfInertiaUnit.CentimeterToTheFourth,
            LengthUnit.Chain => throw new NotImplementedException(),
            LengthUnit.DataMile => throw new NotImplementedException(),
            LengthUnit.Decameter => throw new NotImplementedException(),
            LengthUnit.Decimeter => throw new NotImplementedException(),
            LengthUnit.DtpPica => throw new NotImplementedException(),
            LengthUnit.DtpPoint => throw new NotImplementedException(),
            LengthUnit.Fathom => throw new NotImplementedException(),
            LengthUnit.Femtometer => throw new NotImplementedException(),
            LengthUnit.Foot => AreaMomentOfInertiaUnit.FootToTheFourth,
            LengthUnit.Gigameter => throw new NotImplementedException(),
            LengthUnit.Hand => throw new NotImplementedException(),
            LengthUnit.Hectometer => throw new NotImplementedException(),
            LengthUnit.Inch => AreaMomentOfInertiaUnit.InchToTheFourth,
            LengthUnit.Kilofoot => throw new NotImplementedException(),
            LengthUnit.KilolightYear => throw new NotImplementedException(),
            LengthUnit.Kilometer => throw new NotImplementedException(),
            LengthUnit.Kiloparsec => throw new NotImplementedException(),
            LengthUnit.LightYear => throw new NotImplementedException(),
            LengthUnit.MegalightYear => throw new NotImplementedException(),
            LengthUnit.Megameter => throw new NotImplementedException(),
            LengthUnit.Megaparsec => throw new NotImplementedException(),
            LengthUnit.Meter => AreaMomentOfInertiaUnit.MeterToTheFourth,
            LengthUnit.Microinch => throw new NotImplementedException(),
            LengthUnit.Micrometer => throw new NotImplementedException(),
            LengthUnit.Mil => throw new NotImplementedException(),
            LengthUnit.Mile => throw new NotImplementedException(),
            LengthUnit.Millimeter => AreaMomentOfInertiaUnit.MillimeterToTheFourth,
            LengthUnit.Nanometer => throw new NotImplementedException(),
            LengthUnit.NauticalMile => throw new NotImplementedException(),
            LengthUnit.Parsec => throw new NotImplementedException(),
            LengthUnit.Picometer => throw new NotImplementedException(),
            LengthUnit.PrinterPica => throw new NotImplementedException(),
            LengthUnit.PrinterPoint => throw new NotImplementedException(),
            LengthUnit.Shackle => throw new NotImplementedException(),
            LengthUnit.SolarRadius => throw new NotImplementedException(),
            LengthUnit.Twip => throw new NotImplementedException(),
            LengthUnit.UsSurveyFoot => throw new NotImplementedException(),
            LengthUnit.Yard => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };
    }

    public static ForcePerLengthUnit DivideBy(this ForceUnit forceUnit, LengthUnit lengthUnit)
    {
        var result = forceUnit switch
        {
            ForceUnit.Decanewton => throw new NotImplementedException(),
            ForceUnit.Dyn => throw new NotImplementedException(),
            ForceUnit.KilogramForce => lengthUnit switch
            {
                LengthUnit.Centimeter => ForcePerLengthUnit.KilogramForcePerCentimeter,
                LengthUnit.Meter => ForcePerLengthUnit.KilogramForcePerMeter,
                LengthUnit.Millimeter => ForcePerLengthUnit.KilogramForcePerMillimeter,
                _ => throw new NotImplementedException(
                    $"Unable to handle force unit {forceUnit} with length unit {lengthUnit}."
                ),
            },
            ForceUnit.Kilonewton => lengthUnit switch
            {
                LengthUnit.Centimeter => ForcePerLengthUnit.KilonewtonPerCentimeter,
                LengthUnit.Meter => ForcePerLengthUnit.KilonewtonPerMeter,
                LengthUnit.Millimeter => ForcePerLengthUnit.KilonewtonPerMillimeter,
                _ => throw new NotImplementedException(
                    $"Unable to handle force unit {forceUnit} with length unit {lengthUnit}."
                ),
            },
            ForceUnit.KiloPond => throw new NotImplementedException(),
            ForceUnit.KilopoundForce => lengthUnit switch
            {
                LengthUnit.Foot => ForcePerLengthUnit.KilopoundForcePerFoot,
                LengthUnit.Inch => ForcePerLengthUnit.KilopoundForcePerInch,
                _ => throw new NotImplementedException(
                    $"Unable to handle force unit {forceUnit} with length unit {lengthUnit}."
                ),
            },
            ForceUnit.Meganewton => throw new NotImplementedException(),
            ForceUnit.Micronewton => throw new NotImplementedException(),
            ForceUnit.Millinewton => throw new NotImplementedException(),
            ForceUnit.Newton => lengthUnit switch
            {
                LengthUnit.Centimeter => ForcePerLengthUnit.NewtonPerCentimeter,
                LengthUnit.Meter => ForcePerLengthUnit.NewtonPerMeter,
                LengthUnit.Millimeter => ForcePerLengthUnit.NewtonPerMillimeter,
                _ => throw new NotImplementedException(
                    $"Unable to handle force unit {forceUnit} with length unit {lengthUnit}."
                ),
            },
            ForceUnit.OunceForce => throw new NotImplementedException(),
            ForceUnit.Poundal => throw new NotImplementedException(),
            ForceUnit.PoundForce => lengthUnit switch
            {
                LengthUnit.Foot => ForcePerLengthUnit.PoundForcePerFoot,
                LengthUnit.Inch => ForcePerLengthUnit.PoundForcePerInch,
                _ => throw new NotImplementedException(
                    $"Unable to handle force unit {forceUnit} with length unit {lengthUnit}."
                ),
            },
            ForceUnit.ShortTonForce => throw new NotImplementedException(),
            ForceUnit.TonneForce => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };

        return result;
    }

    public static TorqueUnit MultiplyBy(this ForceUnit forceUnit, LengthUnit lengthUnit)
    {
        return forceUnit switch
        {
            ForceUnit.Decanewton => throw new NotImplementedException(),
            ForceUnit.Dyn => throw new NotImplementedException(),
            ForceUnit.KilogramForce => lengthUnit switch
            {
                LengthUnit.Centimeter => TorqueUnit.KilogramForceCentimeter,
                LengthUnit.Meter => TorqueUnit.KilogramForceMeter,
                LengthUnit.Millimeter => TorqueUnit.KilogramForceMillimeter,
            },
            ForceUnit.Kilonewton => lengthUnit switch
            {
                LengthUnit.Centimeter => TorqueUnit.KilonewtonCentimeter,
                LengthUnit.Meter => TorqueUnit.KilonewtonMeter,
                LengthUnit.Millimeter => TorqueUnit.KilonewtonMillimeter,
            },
            ForceUnit.KiloPond => throw new NotImplementedException(),
            ForceUnit.KilopoundForce => lengthUnit switch
            {
                LengthUnit.Foot => TorqueUnit.KilopoundForceFoot,
                LengthUnit.Inch => TorqueUnit.KilopoundForceInch,
            },
            ForceUnit.Meganewton => throw new NotImplementedException(),
            ForceUnit.Micronewton => throw new NotImplementedException(),
            ForceUnit.Millinewton => throw new NotImplementedException(),
            ForceUnit.Newton => lengthUnit switch
            {
                LengthUnit.Centimeter => TorqueUnit.NewtonCentimeter,
                LengthUnit.Meter => TorqueUnit.NewtonMeter,
                LengthUnit.Millimeter => TorqueUnit.NewtonMillimeter,
            },
            ForceUnit.OunceForce => throw new NotImplementedException(),
            ForceUnit.Poundal => throw new NotImplementedException(),
            ForceUnit.PoundForce => lengthUnit switch
            {
                LengthUnit.Foot => TorqueUnit.PoundForceFoot,
                LengthUnit.Inch => TorqueUnit.PoundForceInch,
            },
            ForceUnit.ShortTonForce => throw new NotImplementedException(),
            ForceUnit.TonneForce => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };
    }

    public static PressureUnit GetPressure(this ForceUnit forceUnit, LengthUnit lengthUnit)
    {
        return forceUnit switch
        {
            ForceUnit.Decanewton => throw new NotImplementedException(),
            ForceUnit.Dyn => throw new NotImplementedException(),
            ForceUnit.KilogramForce => lengthUnit switch
            {
                LengthUnit.Centimeter => PressureUnit.KilogramForcePerSquareCentimeter,
                LengthUnit.Meter => PressureUnit.KilogramForcePerSquareMeter,
                LengthUnit.Millimeter => PressureUnit.KilogramForcePerSquareMillimeter,
            },
            ForceUnit.Kilonewton => lengthUnit switch
            {
                LengthUnit.Centimeter => PressureUnit.KilonewtonPerSquareCentimeter,
                LengthUnit.Meter => PressureUnit.KilonewtonPerSquareMeter,
                LengthUnit.Millimeter => PressureUnit.KilonewtonPerSquareMillimeter,
            },
            ForceUnit.KiloPond => throw new NotImplementedException(),
            ForceUnit.KilopoundForce => lengthUnit switch
            {
                LengthUnit.Foot => PressureUnit.KilopoundForcePerSquareFoot,
                LengthUnit.Inch => PressureUnit.KilopoundForcePerSquareInch,
            },
            ForceUnit.Meganewton => throw new NotImplementedException(),
            ForceUnit.Micronewton => throw new NotImplementedException(),
            ForceUnit.Millinewton => throw new NotImplementedException(),
            ForceUnit.Newton => lengthUnit switch
            {
                LengthUnit.Centimeter => PressureUnit.NewtonPerSquareCentimeter,
                LengthUnit.Meter => PressureUnit.NewtonPerSquareMeter,
                LengthUnit.Millimeter => PressureUnit.NewtonPerSquareMillimeter,
            },
            ForceUnit.OunceForce => throw new NotImplementedException(),
            ForceUnit.Poundal => throw new NotImplementedException(),
            ForceUnit.PoundForce => lengthUnit switch
            {
                LengthUnit.Foot => PressureUnit.PoundForcePerSquareFoot,
                LengthUnit.Inch => PressureUnit.PoundForcePerSquareInch,
            },
            ForceUnit.ShortTonForce => throw new NotImplementedException(),
            ForceUnit.TonneForce => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };
    }
}
