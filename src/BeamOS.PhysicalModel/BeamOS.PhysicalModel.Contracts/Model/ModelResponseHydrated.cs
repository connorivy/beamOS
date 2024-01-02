using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Contracts.Material;
using BeamOS.PhysicalModel.Contracts.Node;
using BeamOS.PhysicalModel.Contracts.PointLoad;
using BeamOS.PhysicalModel.Contracts.SectionProfile;

namespace BeamOS.PhysicalModel.Contracts.Model;

public record ModelResponseHydrated(
    string Id,
    string Name,
    string Description,
    ModelSettingsResponse Settings,
    List<NodeResponse> Nodes,
    List<Element1DResponse> Element1Ds,
    List<MaterialResponse> Materials,
    List<SectionProfileResponse> SectionProfiles,
    List<PointLoadResponse> PointLoads
);
