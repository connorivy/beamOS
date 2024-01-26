using BeamOS.Common.Domain.Enums;
using BeamOS.Common.Domain.Interfaces;
using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate.ValueObjects;

public class AnalyticalNodeId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<AnalyticalNodeId, Guid>
{
    public static AnalyticalNodeId Construct(Guid t1) => new(t1);

    public IEnumerable<UnsupportedStructureDisplacementId> GetUnsupportedStructureDisplacementIds()
    {
        foreach (
            CoordinateSystemDirection3D direction in Enum.GetValues(
                typeof(CoordinateSystemDirection3D)
            )
        )
        {
            if (direction == CoordinateSystemDirection3D.Undefined)
            {
                continue;
            }
            yield return new(this, direction);
        }
    }
}
