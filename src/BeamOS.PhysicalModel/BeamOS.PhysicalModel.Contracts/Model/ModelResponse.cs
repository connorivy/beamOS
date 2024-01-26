using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Contracts.Material;
using BeamOS.PhysicalModel.Contracts.Node;
using BeamOS.PhysicalModel.Contracts.PointLoad;
using BeamOS.PhysicalModel.Contracts.SectionProfile;

namespace BeamOS.PhysicalModel.Contracts.Model;

public record ModelResponse(
    string Id,
    string Name,
    string Description,
    ModelSettingsResponse Settings,
    List<string>? NodeIds = null,
    List<string>? Element1DIds = null,
    List<string>? MaterialIds = null,
    List<string>? SectionProfileIds = null,
    List<NodeResponse>? Nodes = null,
    List<Element1DResponse>? Element1Ds = null,
    List<MaterialResponse>? Materials = null,
    List<SectionProfileResponse>? SectionProfiles = null,
    List<PointLoadResponse>? PointLoads = null
);

public record ModelSettingsResponse(UnitSettingsResponse UnitSettings);

public record UnitSettingsResponse(
    string LengthUnit,
    string AreaUnit,
    string VolumeUnit,
    string ForceUnit,
    string ForcePerLengthUnit,
    string TorqueUnit
)
{
    public static UnitSettingsResponse K_IN { get; } =
        new(
            "Inch",
            "SquareInch",
            "CubicInch",
            "KilopoundForce",
            "KilopoundForcePerInch",
            "KilopoundForceInch"
        );
}
