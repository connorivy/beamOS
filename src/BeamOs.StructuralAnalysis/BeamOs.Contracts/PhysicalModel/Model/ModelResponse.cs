using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.Contracts.PhysicalModel.Model;

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
