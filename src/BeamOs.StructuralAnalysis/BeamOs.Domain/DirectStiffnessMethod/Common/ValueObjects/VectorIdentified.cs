using BeamOs.Domain.AnalyticalModel.Common.ValueObjects;

namespace BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;

public class VectorIdentified(
    List<UnsupportedStructureDisplacementId2> identifiers,
    double[]? values = null
) : VectorIdentifiedGeneric<UnsupportedStructureDisplacementId2>(identifiers, values) { }
