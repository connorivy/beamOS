using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.Element1d;

public record CreateElement1dRequest(
    string ModelId,
    string StartNodeId,
    string EndNodeId,
    string MaterialId,
    string SectionProfileId,
    UnitValueDto? SectionProfileRotation = null,
    Dictionary<string, object>? CustomData = null
);
