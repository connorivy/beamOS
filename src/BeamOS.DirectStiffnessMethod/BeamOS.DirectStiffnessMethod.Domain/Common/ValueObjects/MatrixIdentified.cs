using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate.ValueObjects;

namespace BeamOS.DirectStiffnessMethod.Domain.Common.ValueObjects;

public class MatrixIdentified(
    List<UnsupportedStructureDisplacementId> identifiers,
    double[,]? values = null
) : MatrixIdentifiedGeneric<UnsupportedStructureDisplacementId>(identifiers, values) { }
