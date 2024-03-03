using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.Element1d;

public record Element1DResponse(
    string Id,
    string ModelId,
    string StartNodeId,
    string EndNodeId,
    string MaterialId,
    string SectionProfileId,
    UnitValueDto SectionProfileRotation
);
