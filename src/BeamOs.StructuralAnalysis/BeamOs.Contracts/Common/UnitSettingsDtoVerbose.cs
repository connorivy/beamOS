namespace BeamOs.Contracts.Common;

public record UnitSettingsDtoVerbose(
    string LengthUnit,
    string AreaUnit,
    string VolumeUnit,
    string AreaMomentOfInertiaUnit,
    string ForceUnit,
    string TorqueUnit,
    string ForcePerLengthUnit,
    string PressureUnit,
    string AngleUnit = "Radian"
)
{
    public static UnitSettingsDtoVerbose K_IN { get; } =
        new(
            "Inch",
            "SquareInch",
            "CubicInch",
            "InchToTheFourth",
            "KilopoundForce",
            "KilopoundForceInch",
            "KilopoundForcePerInch",
            "KilopoundForcePerSquareInch"
        );

    public static UnitSettingsDtoVerbose K_FT { get; } =
        new(
            "Foot",
            "SquareFoot",
            "CubicFoot",
            "FootToTheFourth",
            "KilopoundForce",
            "KilopoundForceFoot",
            "KilopoundForcePerFoot",
            "KilopoundForcePerSquareFoot"
        );

    public static UnitSettingsDtoVerbose SI { get; } =
        new(
            "Meter",
            "SquareMeter",
            "CubicMeter",
            "MeterToTheFourth",
            "Newton",
            "NewtonMeter",
            "NewtonPerMeter",
            "NewtonPerSquareMeter"
        );

    public static UnitSettingsDtoVerbose kN_M { get; } =
        new(
            "Meter",
            "SquareMeter",
            "CubicMeter",
            "MeterToTheFourth",
            "Kilonewton",
            "KilonewtonMeter",
            "KilonewtonPerMeter",
            "KilonewtonPerSquareMeter"
        );
}

public enum PreconfiguredUnits
{
    N_M = 0,
    K_IN = 1,
    K_FT = 2,
}

public record UnitSettingsContract
{
    public required string LengthUnit { get; init; }
    public required string ForceUnit { get; init; }
    public string AngleUnit { get; init; } = "Radian";

    public static UnitSettingsContract K_IN { get; } =
        new() { LengthUnit = "Inch", ForceUnit = "KilopoundForce" };

    public static UnitSettingsContract K_FT { get; } =
        new() { LengthUnit = "Foot", ForceUnit = "KilopoundForce" };

    public static UnitSettingsContract SI { get; } =
        new() { LengthUnit = "Meter", ForceUnit = "Newton" };

    public static UnitSettingsContract kN_M { get; } =
        new() { LengthUnit = "Meter", ForceUnit = "Kilonewton" };
}
