using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.Extensions;

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
