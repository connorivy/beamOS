using System.Text.Json.Serialization;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

public record UnitSettings
{
    public required LengthUnitContract LengthUnit { get; init; }
    public required ForceUnitContract ForceUnit { get; init; }
    public AngleUnitContract AngleUnit { get; init; } = AngleUnitContract.Radian;

    public static UnitSettings K_IN { get; } =
        new()
        {
            LengthUnit = LengthUnitContract.Inch,
            ForceUnit = ForceUnitContract.KilopoundForce,
        };

    public static UnitSettings K_FT { get; } =
        new()
        {
            LengthUnit = LengthUnitContract.Foot,
            ForceUnit = ForceUnitContract.KilopoundForce,
        };

    public static UnitSettings N_M { get; } =
        new() { LengthUnit = LengthUnitContract.Meter, ForceUnit = ForceUnitContract.Newton };

#pragma warning disable IDE1006 // Naming Styles
    public static UnitSettings kN_M { get; } =
#pragma warning restore IDE1006 // Naming Styles
        new() { LengthUnit = LengthUnitContract.Meter, ForceUnit = ForceUnitContract.Kilonewton };

    [JsonIgnore]
    public AreaUnitContract AreaUnit => this.LengthUnit.ToArea();

    [JsonIgnore]
    public VolumeUnitContract VolumeUnit => this.LengthUnit.ToVolume();

    [JsonIgnore]
    public AreaMomentOfInertiaUnitContract AreaMomentOfInertiaUnit =>
        this.LengthUnit.ToAreaMomentOfInertia();

    [JsonIgnore]
    public TorqueUnitContract TorqueUnit => this.ForceUnit.MultiplyBy(this.LengthUnit);

    [JsonIgnore]
    public ForcePerLengthUnitContract ForcePerLengthUnit =>
        this.ForceUnit.DivideBy(this.LengthUnit);

    [JsonIgnore]
    public PressureUnitContract PressureUnit => this.ForceUnit.GetPressure(this.LengthUnit);
}

public enum LengthUnit
{
    Undefined = 0,
    Centimeter,
    Foot,
    Inch,
    Meter,
    Millimeter,
}

public enum AreaUnit
{
    Undefined = 0,
    SquareCentimeter,
    SquareFoot,
    SquareInch,
    SquareMeter,
    SquareMillimeter,
}

public enum VolumeUnit
{
    Undefined = 0,
    CubicCentimeter,
    CubicFoot,
    CubicInch,
    CubicMeter,
    CubicMillimeter,
}

public enum AreaMomentOfInertiaUnit
{
    Undefined = 0,
    CentimeterToTheFourth,
    FootToTheFourth,
    InchToTheFourth,
    MeterToTheFourth,
    MillimeterToTheFourth,
}

public enum ForceUnit
{
    Undefined = 0,
    Kilonewton,
    KilopoundForce,
    Newton,
    PoundForce,
}

public enum AngleUnit
{
    Undefined = 0,
    Degree,
    Radian,
}

public enum TorqueUnit
{
    Undefined = 0,

    //GramForceCentimeter = 1,
    //GramForceMeter = 2,
    //GramForceMillimeter = 3,
    //KilogramForceCentimeter = 4,
    //KilogramForceMeter = 5,
    //KilogramForceMillimeter = 6,
    KilonewtonCentimeter = 7,
    KilonewtonMeter = 8,
    KilonewtonMillimeter = 9,
    KilopoundForceFoot = 10,
    KilopoundForceInch = 11,

    //MeganewtonCentimeter = 12,
    //MeganewtonMeter = 13,
    //MeganewtonMillimeter = 14,
    //MegapoundForceFoot = 15,
    //MegapoundForceInch = 16,
    NewtonCentimeter = 17,
    NewtonMeter = 18,
    NewtonMillimeter = 19,
    PoundForceFoot = 21,
    PoundForceInch = 22,
    //TonneForceCentimeter = 23,
    //TonneForceMeter = 24,
    //TonneForceMillimeter = 25,
}

public enum ForcePerLengthUnit
{
    Undefined = 0,
    KilonewtonPerCentimeter = 7,
    KilonewtonPerMeter = 8,
    KilonewtonPerMillimeter = 9,
    KilopoundForcePerFoot = 10,
    KilopoundForcePerInch = 11,
    NewtonPerCentimeter = 17,
    NewtonPerMeter = 18,
    NewtonPerMillimeter = 19,
    PoundForcePerFoot = 21,
    PoundForcePerInch = 22,
}

public enum PressureUnit
{
    Undefined = 0,
    KilonewtonPerSquareCentimeter = 7,
    KilonewtonPerSquareMeter = 8,
    KilonewtonPerSquareMillimeter = 9,
    KilopoundForcePerSquareFoot = 10,
    KilopoundForcePerSquareInch = 11,
    NewtonPerSquareCentimeter = 17,
    NewtonPerSquareMeter = 18,
    NewtonPerSquareMillimeter = 19,
    PoundForcePerSquareFoot = 21,
    PoundForcePerSquareInch = 22,
}

public enum RatioUnit
{
    Undefined = 0,
    DecimalFraction, // 0 to 1
    Percent, // 0 to 100
}

public static class PressureUnitExtension
{
    public static string ToFriendlyString(this AreaUnitContract areaUnit) =>
        areaUnit switch
        {
            AreaUnitContract.SquareCentimeter => "cm²",
            AreaUnitContract.SquareFoot => "ft²",
            AreaUnitContract.SquareInch => "in²",
            AreaUnitContract.SquareMeter => "m²",
            AreaUnitContract.SquareMillimeter => "mm²",
            AreaUnitContract.Undefined => throw new NotImplementedException(),
            _ => areaUnit.ToString(),
        };

    public static string ToFriendlyString(
        this AreaMomentOfInertiaUnitContract areaMomentOfInertiaUnit
    ) =>
        areaMomentOfInertiaUnit switch
        {
            AreaMomentOfInertiaUnitContract.CentimeterToTheFourth => "cm⁴",
            AreaMomentOfInertiaUnitContract.FootToTheFourth => "ft⁴",
            AreaMomentOfInertiaUnitContract.InchToTheFourth => "in⁴",
            AreaMomentOfInertiaUnitContract.MeterToTheFourth => "m⁴",
            AreaMomentOfInertiaUnitContract.MillimeterToTheFourth => "mm⁴",
            AreaMomentOfInertiaUnitContract.Undefined => throw new NotImplementedException(),
            _ => areaMomentOfInertiaUnit.ToString(),
        };

    public static string ToFriendlyString(this LengthUnitContract lengthUnit) =>
        lengthUnit switch
        {
            LengthUnitContract.Centimeter => "cm",
            LengthUnitContract.Foot => "ft",
            LengthUnitContract.Inch => "in",
            LengthUnitContract.Meter => "m",
            LengthUnitContract.Millimeter => "mm",
            LengthUnitContract.Undefined => throw new NotImplementedException(),
            _ => lengthUnit.ToString(),
        };

    public static string ToFriendlyString(this VolumeUnitContract volumeUnit) =>
        volumeUnit switch
        {
            VolumeUnitContract.CubicCentimeter => "cm³",
            VolumeUnitContract.CubicFoot => "ft³",
            VolumeUnitContract.CubicInch => "in³",
            VolumeUnitContract.CubicMeter => "m³",
            VolumeUnitContract.CubicMillimeter => "mm³",
            VolumeUnitContract.Undefined => throw new NotImplementedException(),
            _ => volumeUnit.ToString(),
        };

    public static string ToFriendlyString(this TorqueUnitContract torqueUnit) =>
        torqueUnit switch
        {
            TorqueUnitContract.KilonewtonCentimeter => "kN·cm",
            TorqueUnitContract.KilonewtonMeter => "kN·m",
            TorqueUnitContract.KilonewtonMillimeter => "kN·mm",
            TorqueUnitContract.KilopoundForceFoot => "kip·ft",
            TorqueUnitContract.KilopoundForceInch => "kip·in",
            TorqueUnitContract.NewtonCentimeter => "N·cm",
            TorqueUnitContract.NewtonMeter => "N·m",
            TorqueUnitContract.NewtonMillimeter => "N·mm",
            TorqueUnitContract.PoundForceFoot => "lb·ft",
            TorqueUnitContract.PoundForceInch => "lb·in",
            TorqueUnitContract.Undefined => throw new NotImplementedException(),
            _ => torqueUnit.ToString(),
        };

    public static string ToFriendlyString(this PressureUnitContract pressureUnit) =>
        pressureUnit switch
        {
            PressureUnitContract.KilonewtonPerSquareCentimeter => "kN/cm²",
            PressureUnitContract.KilonewtonPerSquareMeter => "kN/m²",
            PressureUnitContract.KilonewtonPerSquareMillimeter => "kN/mm²",
            PressureUnitContract.KilopoundForcePerSquareFoot => "kip/ft²",
            PressureUnitContract.KilopoundForcePerSquareInch => "kip/in²",
            PressureUnitContract.NewtonPerSquareCentimeter => "N/cm²",
            PressureUnitContract.NewtonPerSquareMeter => "N/m²",
            PressureUnitContract.NewtonPerSquareMillimeter => "N/mm²",
            PressureUnitContract.PoundForcePerSquareFoot => "lb/ft²",
            PressureUnitContract.PoundForcePerSquareInch => "lb/in²",
            PressureUnitContract.Undefined => throw new NotImplementedException(),
            _ => pressureUnit.ToString(),
        };
}
