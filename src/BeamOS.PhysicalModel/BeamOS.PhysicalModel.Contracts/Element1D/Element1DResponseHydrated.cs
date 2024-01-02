using BeamOS.Common.Contracts;
using BeamOS.PhysicalModel.Contracts.Material;
using BeamOS.PhysicalModel.Contracts.Node;
using BeamOS.PhysicalModel.Contracts.SectionProfile;

namespace BeamOS.PhysicalModel.Contracts.Element1D;

public record Element1DResponseHydrated(
    string Id,
    string ModelId,
    NodeResponse StartNode,
    NodeResponse EndNode,
    MaterialResponse Material,
    SectionProfileResponse SectionProfile,
    UnitValueDTO SectionProfileRotation
);
