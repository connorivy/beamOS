using BeamOs.Domain.AnalyticalResults.Common.ValueObjects;

namespace BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;

public class MatrixIdentified(
    List<UnsupportedStructureDisplacementId2> identifiers,
    double[,]? values = null
) : MatrixIdentifiedGeneric<UnsupportedStructureDisplacementId2>(identifiers, values) { }
