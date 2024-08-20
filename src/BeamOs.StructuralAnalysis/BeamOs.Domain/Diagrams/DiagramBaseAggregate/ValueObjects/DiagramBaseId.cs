using BeamOs.Common.Domain.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.Diagrams.DiagramBaseAggregate.ValueObjects;

public class DiagramBaseId(Guid? id = null) : GuidBasedId(id), IConstructable<DiagramBaseId, Guid>
{
    public static DiagramBaseId Construct(Guid t1) => new(t1);
}
