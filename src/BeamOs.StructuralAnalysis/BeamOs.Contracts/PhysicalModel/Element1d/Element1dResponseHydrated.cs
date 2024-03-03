using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.Contracts.PhysicalModel.Element1d;

public record Element1DResponseHydrated(
    string Id,
    string ModelId,
    NodeResponse StartNode,
    NodeResponse EndNode,
    MaterialResponse Material,
    SectionProfileResponse SectionProfile,
    UnitValueDto SectionProfileRotation
);
