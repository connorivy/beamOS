using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;

public record Element1dResponse(
    int Id,
    Guid ModelId,
    int StartNodeId,
    int EndNodeId,
    int MaterialId,
    int SectionProfileId,
    AngleContract SectionProfileRotation,
    Dictionary<string, string>? Metadata = null
) : IModelEntity;
