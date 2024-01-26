using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate.ValueObjects;

namespace BeamOS.DirectStiffnessMethod.Domain.Common.ValueObjects;

public class VectorIdentified(
    List<UnsupportedStructureDisplacementId> identifiers,
    double[]? values = null
) : VectorIdentifiedGeneric<UnsupportedStructureDisplacementId>(identifiers, values) { }
