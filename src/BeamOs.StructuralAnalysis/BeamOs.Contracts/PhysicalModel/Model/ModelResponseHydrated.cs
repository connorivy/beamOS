using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Contracts.PhysicalModel.Common.Interfaces;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Contracts.PhysicalModel.MomentLoad;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.Contracts.PhysicalModel.Model;

public record ModelResponseHydrated(
    string Id,
    string Name,
    string Description,
    ModelSettingsResponse Settings,
    List<NodeResponse> Nodes,
    List<Element1DResponse> Element1Ds,
    List<MaterialResponse> Materials,
    List<SectionProfileResponse> SectionProfiles,
    List<PointLoadResponse> PointLoads,
    List<MomentLoadResponse> MomentLoads
) : BeamOsContractBase, IHasId;
