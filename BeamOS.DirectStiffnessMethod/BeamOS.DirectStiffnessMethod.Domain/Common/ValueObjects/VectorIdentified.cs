using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate.ValueObjects;

namespace BeamOS.DirectStiffnessMethod.Domain.Common.ValueObjects;
public class VectorIdentified : VectorIdentifiedGeneric<UnsupportedStructureDisplacementId>
{
    public VectorIdentified(List<UnsupportedStructureDisplacementId> identifiers) : base(identifiers)
    {
    }

    public VectorIdentified(List<UnsupportedStructureDisplacementId> identifiers, double[] values)
        : base(identifiers, values) { }
}
