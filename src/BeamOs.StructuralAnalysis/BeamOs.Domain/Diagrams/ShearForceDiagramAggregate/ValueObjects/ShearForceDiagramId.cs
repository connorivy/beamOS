using BeamOs.Common.Domain.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.Diagrams.ShearForceDiagramAggregate.ValueObjects;

public class ShearForceDiagramId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<ShearForceDiagramId, Guid>
{
    public static ShearForceDiagramId Construct(Guid t1) => new(t1);
}
