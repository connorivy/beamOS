using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.Model;

public record CreateModelRequest(
    string Name,
    string Description,
    PhysicalModelSettingsDto Settings
);

public record PhysicalModelSettingsDto(UnitSettingsDtoVerbose UnitSettings);

//public record UnitSettingsRequest(
//    string LengthUnit,
//    string AreaUnit,
//    string VolumeUnit,
//    string ForceUnit,
//    string ForcePerLengthUnit,
//    string TorqueUnit,
//    string PressureUnit,
//    string AreaMomentOfInertiaUnit
//)
//{
//    public static UnitSettingsRequest K_IN { get; } =
//        new(
//            "Inch",
//            "SquareInch",
//            "CubicInch",
//            "KilopoundForce",
//            "KilopoundForcePerInch",
//            "KilopoundForceInch",
//            "KilopoundForcePerSquareInch",
//            "InchToTheFourth"
//        );

//    public static UnitSettingsRequest SI { get; } =
//        new(
//            "Meter",
//            "SquareMeter",
//            "CubicMeter",
//            "Newton",
//            "NewtonPerMeter",
//            "NewtonMeter",
//            "NewtonPerSquareMeter",
//            "MeterToTheFourth"
//        );
//}
