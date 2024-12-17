using System.Text.Json.Serialization;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

public record UnitSettingsContract
{
    public required LengthUnitContract LengthUnit { get; init; }
    public required ForceUnitContract ForceUnit { get; init; }
    public AngleUnitContract AngleUnit { get; init; } = AngleUnitContract.Radian;

    public static UnitSettingsContract K_IN { get; } =
        new()
        {
            LengthUnit = LengthUnitContract.Inch,
            ForceUnit = ForceUnitContract.KilopoundForce,
        };

    public static UnitSettingsContract K_FT { get; } =
        new()
        {
            LengthUnit = LengthUnitContract.Foot,
            ForceUnit = ForceUnitContract.KilopoundForce,
        };

    public static UnitSettingsContract N_M { get; } =
        new() { LengthUnit = LengthUnitContract.Meter, ForceUnit = ForceUnitContract.Newton };

    public static UnitSettingsContract kN_M { get; } =
        new() { LengthUnit = LengthUnitContract.Meter, ForceUnit = ForceUnitContract.Kilonewton };

    [JsonIgnore]
    public AreaUnitContract AreaUnit => LengthUnit.ToArea();

    [JsonIgnore]
    public VolumeUnitContract VolumeUnit => LengthUnit.ToVolume();

    [JsonIgnore]
    public AreaMomentOfInertiaUnitContract AreaMomentOfInertiaUnit =>
        LengthUnit.ToAreaMomentOfInertia();

    [JsonIgnore]
    public TorqueUnitContract TorqueUnit => ForceUnit.MultiplyBy(LengthUnit);

    [JsonIgnore]
    public ForcePerLengthUnitContract ForcePerLengthUnit => ForceUnit.DivideBy(LengthUnit);

    [JsonIgnore]
    public PressureUnitContract PressureUnit => ForceUnit.GetPressure(LengthUnit);
}

public enum LengthUnitContract
{
    Undefined = 0,
    Centimeter,
    Foot,
    Inch,
    Meter,
    Millimeter,
}

public enum AreaUnitContract
{
    Undefined = 0,
    SquareCentimeter,
    SquareFoot,
    SquareInch,
    SquareMeter,
    SquareMillimeter,
}

public enum VolumeUnitContract
{
    Undefined = 0,
    CubicCentimeter,
    CubicFoot,
    CubicInch,
    CubicMeter,
    CubicMillimeter,
}

public enum AreaMomentOfInertiaUnitContract
{
    Undefined = 0,
    CentimeterToTheFourth,
    FootToTheFourth,
    InchToTheFourth,
    MeterToTheFourth,
    MillimeterToTheFourth,
}

public enum ForceUnitContract
{
    Undefined = 0,
    Kilonewton,
    KilopoundForce,
    Newton,
    PoundForce,
}

public enum AngleUnitContract
{
    Undefined = 0,
    Degree,
    Radian,
}

public enum TorqueUnitContract
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

public enum ForcePerLengthUnitContract
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

public enum PressureUnitContract
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
