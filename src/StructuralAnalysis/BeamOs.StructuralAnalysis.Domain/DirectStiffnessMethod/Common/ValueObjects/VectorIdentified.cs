namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.ValueObjects;

public class VectorIdentified(
    List<UnsupportedStructureDisplacementId2> identifiers,
    double[]? values = null
) : VectorIdentifiedGeneric<UnsupportedStructureDisplacementId2>(identifiers, values) { }
