using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.Contracts.PhysicalModel.Element1d;

public record Element1dResponseHydrated(
    string Id,
    string ModelId,
    string StartNodeId,
    string EndNodeId,
    string MaterialId,
    string SectionProfileId,
    UnitValueDto SectionProfileRotation,
    NodeResponse? StartNode,
    NodeResponse? EndNode,
    MaterialResponse? Material,
    SectionProfileResponse? SectionProfile
);
