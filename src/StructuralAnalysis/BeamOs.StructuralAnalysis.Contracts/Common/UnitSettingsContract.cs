using System.Text.Json.Serialization;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

public record UnitSettings
{
    public required LengthUnit LengthUnit { get; init; }
    public required ForceUnit ForceUnit { get; init; }
    public AngleUnit AngleUnit { get; init; } = AngleUnit.Radian;

    // public RatioUnit RatioUnit { get; init; } = RatioUnit.DecimalFraction;

    public static UnitSettings K_IN { get; } =
        new() { LengthUnit = LengthUnit.Inch, ForceUnit = ForceUnit.KilopoundForce };

    public static UnitSettings K_FT { get; } =
        new() { LengthUnit = LengthUnit.Foot, ForceUnit = ForceUnit.KilopoundForce };

    public static UnitSettings N_M { get; } =
        new() { LengthUnit = LengthUnit.Meter, ForceUnit = ForceUnit.Newton };

#pragma warning disable IDE1006 // Naming Styles
    public static UnitSettings kN_M { get; } =
#pragma warning restore IDE1006 // Naming Styles
        new() { LengthUnit = LengthUnit.Meter, ForceUnit = ForceUnit.Kilonewton };

    [JsonIgnore]
    public AreaUnit AreaUnit => this.LengthUnit.ToArea();

    [JsonIgnore]
    public VolumeUnit VolumeUnit => this.LengthUnit.ToVolume();

    [JsonIgnore]
    public AreaMomentOfInertiaUnit AreaMomentOfInertiaUnit =>
        this.LengthUnit.ToAreaMomentOfInertia();

    [JsonIgnore]
    public TorqueUnit TorqueUnit => this.ForceUnit.MultiplyBy(this.LengthUnit);

    [JsonIgnore]
    public ForcePerLengthUnit ForcePerLengthUnit => this.ForceUnit.DivideBy(this.LengthUnit);

    [JsonIgnore]
    public PressureUnit PressureUnit => this.ForceUnit.GetPressure(this.LengthUnit);
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
    public static string ToFriendlyString(this AreaUnit areaUnit) =>
        areaUnit switch
        {
            AreaUnit.SquareCentimeter => "cm²",
            AreaUnit.SquareFoot => "ft²",
            AreaUnit.SquareInch => "in²",
            AreaUnit.SquareMeter => "m²",
            AreaUnit.SquareMillimeter => "mm²",
            AreaUnit.Undefined => throw new NotImplementedException(),
            _ => areaUnit.ToString(),
        };

    public static string ToFriendlyString(this AreaMomentOfInertiaUnit areaMomentOfInertiaUnit) =>
        areaMomentOfInertiaUnit switch
        {
            AreaMomentOfInertiaUnit.CentimeterToTheFourth => "cm⁴",
            AreaMomentOfInertiaUnit.FootToTheFourth => "ft⁴",
            AreaMomentOfInertiaUnit.InchToTheFourth => "in⁴",
            AreaMomentOfInertiaUnit.MeterToTheFourth => "m⁴",
            AreaMomentOfInertiaUnit.MillimeterToTheFourth => "mm⁴",
            AreaMomentOfInertiaUnit.Undefined => throw new NotImplementedException(),
            _ => areaMomentOfInertiaUnit.ToString(),
        };

    public static string ToFriendlyString(this LengthUnit lengthUnit) =>
        lengthUnit switch
        {
            LengthUnit.Centimeter => "cm",
            LengthUnit.Foot => "ft",
            LengthUnit.Inch => "in",
            LengthUnit.Meter => "m",
            LengthUnit.Millimeter => "mm",
            LengthUnit.Undefined => throw new NotImplementedException(),
            _ => lengthUnit.ToString(),
        };

    public static string ToFriendlyString(this VolumeUnit volumeUnit) =>
        volumeUnit switch
        {
            VolumeUnit.CubicCentimeter => "cm³",
            VolumeUnit.CubicFoot => "ft³",
            VolumeUnit.CubicInch => "in³",
            VolumeUnit.CubicMeter => "m³",
            VolumeUnit.CubicMillimeter => "mm³",
            VolumeUnit.Undefined => throw new NotImplementedException(),
            _ => volumeUnit.ToString(),
        };

    public static string ToFriendlyString(this TorqueUnit torqueUnit) =>
        torqueUnit switch
        {
            TorqueUnit.KilonewtonCentimeter => "kN·cm",
            TorqueUnit.KilonewtonMeter => "kN·m",
            TorqueUnit.KilonewtonMillimeter => "kN·mm",
            TorqueUnit.KilopoundForceFoot => "kip·ft",
            TorqueUnit.KilopoundForceInch => "kip·in",
            TorqueUnit.NewtonCentimeter => "N·cm",
            TorqueUnit.NewtonMeter => "N·m",
            TorqueUnit.NewtonMillimeter => "N·mm",
            TorqueUnit.PoundForceFoot => "lb·ft",
            TorqueUnit.PoundForceInch => "lb·in",
            TorqueUnit.Undefined => throw new NotImplementedException(),
            _ => torqueUnit.ToString(),
        };

    public static string ToFriendlyString(this PressureUnit pressureUnit) =>
        pressureUnit switch
        {
            PressureUnit.KilonewtonPerSquareCentimeter => "kN/cm²",
            PressureUnit.KilonewtonPerSquareMeter => "kN/m²",
            PressureUnit.KilonewtonPerSquareMillimeter => "kN/mm²",
            PressureUnit.KilopoundForcePerSquareFoot => "kip/ft²",
            PressureUnit.KilopoundForcePerSquareInch => "kip/in²",
            PressureUnit.NewtonPerSquareCentimeter => "N/cm²",
            PressureUnit.NewtonPerSquareMeter => "N/m²",
            PressureUnit.NewtonPerSquareMillimeter => "N/mm²",
            PressureUnit.PoundForcePerSquareFoot => "lb/ft²",
            PressureUnit.PoundForcePerSquareInch => "lb/in²",
            PressureUnit.Undefined => throw new NotImplementedException(),
            _ => pressureUnit.ToString(),
        };
}
