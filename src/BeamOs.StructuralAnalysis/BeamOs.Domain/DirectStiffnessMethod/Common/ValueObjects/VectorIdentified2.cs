using BeamOs.Domain.AnalyticalResults.Common.ValueObjects;

namespace BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;

public class VectorIdentified2(
    List<UnsupportedStructureDisplacementId2> identifiers,
    double[]? values = null
) : VectorIdentifiedGeneric<UnsupportedStructureDisplacementId2>(identifiers, values) { }
