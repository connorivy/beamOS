using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;

public record Element1dResponse(
    int Id,
    Guid ModelId,
    int StartNodeId,
    int EndNodeId,
    int MaterialId,
    int SectionProfileId,
    Angle SectionProfileRotation,
    Dictionary<string, string>? Metadata = null
) : IModelEntity
{
    public Element1dData ToElement1dData()
    {
        return new Element1dData(
            this.StartNodeId,
            this.EndNodeId,
            this.MaterialId,
            this.SectionProfileId,
            this.SectionProfileRotation,
            this.Metadata
        );
    }
}
