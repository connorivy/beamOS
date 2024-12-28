using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;

public record CreateElement1dRequest(
    int StartNodeId,
    int EndNodeId,
    int MaterialId,
    int SectionProfileId,
    AngleContract? SectionProfileRotation = null,
    int? Id = null,
    Dictionary<string, string>? Metadata = null
);
