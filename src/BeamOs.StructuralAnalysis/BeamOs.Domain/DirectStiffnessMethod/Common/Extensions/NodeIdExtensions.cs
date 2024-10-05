using BeamOs.Domain.AnalyticalModel.Common.ValueObjects;
using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;

namespace BeamOs.Domain.DirectStiffnessMethod.Common.Extensions;

public static class NodeIdExtensions
{
    public static IEnumerable<UnsupportedStructureDisplacementId2> GetUnsupportedStructureDisplacementIds(
        this NodeId nodeId
    )
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
            yield return new(nodeId, direction);
        }
    }
}
