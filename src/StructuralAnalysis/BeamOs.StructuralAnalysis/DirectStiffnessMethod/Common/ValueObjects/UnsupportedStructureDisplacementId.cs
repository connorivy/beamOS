using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.ValueObjects;

internal record struct UnsupportedStructureDisplacementId(
    NodeId NodeId,
    CoordinateSystemDirection3D Direction
) { }
