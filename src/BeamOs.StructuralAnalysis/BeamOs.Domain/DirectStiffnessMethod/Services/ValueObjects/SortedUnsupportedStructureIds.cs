using BeamOs.Domain.Common.Models;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate.ValueObjects;

namespace BeamOs.Domain.DirectStiffnessMethod.Services.ValueObjects;

public class SortedUnsupportedStructureIds(
    List<UnsupportedStructureDisplacementId> degreeOfFreedomIds,
    List<UnsupportedStructureDisplacementId> boundaryConditionIds
) : BeamOSValueObject
{
    public List<UnsupportedStructureDisplacementId> DegreeOfFreedomIds { get; } =
        degreeOfFreedomIds;
    public List<UnsupportedStructureDisplacementId> BoundaryConditionIds { get; } =
        boundaryConditionIds;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.DegreeOfFreedomIds;
        yield return this.BoundaryConditionIds;
    }
}
