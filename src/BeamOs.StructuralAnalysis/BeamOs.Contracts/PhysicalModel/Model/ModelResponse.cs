using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Contracts.PhysicalModel.MomentLoad;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.Contracts.PhysicalModel.Model;

public record ModelResponse(
    string Id,
    string Name,
    string Description,
    PhysicalModelSettings Settings,
    List<NodeResponse>? Nodes = null,
    List<Element1DResponse>? Element1ds = null,
    List<MaterialResponse>? Materials = null,
    List<SectionProfileResponse>? SectionProfiles = null,
    List<PointLoadResponse>? PointLoads = null,
    List<MomentLoadResponse>? MomentLoads = null
) : BeamOsEntityContractBase(Id);

public record ModelSettingsResponse(UnitSettingsResponse UnitSettings);

public record UnitSettingsResponse(
    string LengthUnit,
    string AreaUnit,
    string VolumeUnit,
    string ForceUnit,
    string ForcePerLengthUnit,
    string TorqueUnit,
    string PressureUnit,
    string AreaMomentOfInertiaUnit
)
{
    public static UnitSettingsResponse K_IN { get; } =
        new(
            "Inch",
            "SquareInch",
            "CubicInch",
            "KilopoundForce",
            "KilopoundForcePerInch",
            "KilopoundForceInch",
            "KilopoundForcePerSquareInch",
            "InchToTheFourth"
        );
}
