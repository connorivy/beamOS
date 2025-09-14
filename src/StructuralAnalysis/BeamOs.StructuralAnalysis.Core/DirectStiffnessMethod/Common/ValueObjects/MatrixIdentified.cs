namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.ValueObjects;

internal class MatrixIdentified(
    List<UnsupportedStructureDisplacementId> identifiers,
    double[,]? values = null
) : MatrixIdentifiedGeneric<UnsupportedStructureDisplacementId>(identifiers, values) { }
