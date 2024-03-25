using BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate.ValueObjects;

namespace BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;

public class VectorIdentified(
    List<UnsupportedStructureDisplacementId> identifiers,
    double[]? values = null
) : VectorIdentifiedGeneric<UnsupportedStructureDisplacementId>(identifiers, values) { }
