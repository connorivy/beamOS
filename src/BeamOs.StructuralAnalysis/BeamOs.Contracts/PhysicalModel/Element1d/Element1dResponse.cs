using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.Contracts.PhysicalModel.Element1d;

public record Element1DResponse(
    string Id,
    string ModelId,
    string StartNodeId,
    string EndNodeId,
    string MaterialId,
    string SectionProfileId,
    UnitValueDto SectionProfileRotation,
    NodeResponse? StartNode = null,
    NodeResponse? EndNode = null,
    MaterialResponse? Material = null,
    SectionProfileResponse? SectionProfile = null
);
