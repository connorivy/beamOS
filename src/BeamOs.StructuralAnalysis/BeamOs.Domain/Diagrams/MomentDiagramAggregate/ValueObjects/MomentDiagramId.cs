using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.Diagrams.MomentDiagramAggregate.ValueObjects;

public class MomentDiagramId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<MomentDiagramId, Guid>
{
    public static MomentDiagramId Construct(Guid t1) => new(t1);
}
