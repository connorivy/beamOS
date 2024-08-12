using BeamOs.Common.Domain.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.Diagrams.Common.ValueObjects;

public sealed class DiagramConsistantIntervalId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<DiagramConsistantIntervalId, Guid>
{
    public static DiagramConsistantIntervalId Construct(Guid t1) => new(t1);
}
