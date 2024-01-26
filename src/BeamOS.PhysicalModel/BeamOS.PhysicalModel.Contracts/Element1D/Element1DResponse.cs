using BeamOS.Common.Contracts;

namespace BeamOS.PhysicalModel.Contracts.Element1D;

public record Element1DResponse(
    string Id,
    string ModelId,
    string StartNodeId,
    string EndNodeId,
    string MaterialId,
    string SectionProfileId,
    UnitValueDTO SectionProfileRotation
);
