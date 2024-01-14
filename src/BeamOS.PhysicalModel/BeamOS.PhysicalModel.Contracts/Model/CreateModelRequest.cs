namespace BeamOS.PhysicalModel.Contracts.Model;

public record CreateModelRequest(string Name, string Description, ModelSettingsRequest Settings);

public record ModelSettingsRequest(UnitSettingsRequest UnitSettings);

public record UnitSettingsRequest(
    string LengthUnit,
    string AreaUnit,
    string VolumeUnit,
    string ForceUnit,
    string ForcePerLengthUnit,
    string TorqueUnit
)
{
    public static UnitSettingsRequest K_IN { get; } =
        new(
            "Inch",
            "InchesSquared",
            "InchesCubed",
            "Kilopounds",
            "KilopoundsPerInch",
            "KilopoundInch"
        );
}
