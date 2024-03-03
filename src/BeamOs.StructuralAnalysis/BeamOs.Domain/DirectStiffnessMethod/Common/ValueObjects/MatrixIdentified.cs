using BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate.ValueObjects;

namespace BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;

public class MatrixIdentified(
    List<UnsupportedStructureDisplacementId> identifiers,
    double[,]? values = null
) : MatrixIdentifiedGeneric<UnsupportedStructureDisplacementId>(identifiers, values) { }
