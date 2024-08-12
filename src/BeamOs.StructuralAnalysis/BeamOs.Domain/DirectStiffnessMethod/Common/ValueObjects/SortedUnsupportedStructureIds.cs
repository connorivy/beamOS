using BeamOs.Common.Domain.Models;
using BeamOs.Domain.AnalyticalResults.Common.ValueObjects;

namespace BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;

public class SortedUnsupportedStructureIds(
    List<UnsupportedStructureDisplacementId2> degreeOfFreedomIds,
    List<UnsupportedStructureDisplacementId2> boundaryConditionIds
) : BeamOSValueObject
{
    public List<UnsupportedStructureDisplacementId2> DegreeOfFreedomIds { get; } =
        degreeOfFreedomIds;
    public List<UnsupportedStructureDisplacementId2> BoundaryConditionIds { get; } =
        boundaryConditionIds;

    public void Deconstruct(
        out List<UnsupportedStructureDisplacementId2> degreeOfFreedomIds,
        out List<UnsupportedStructureDisplacementId2> boundaryConditionIds
    )
    {
        degreeOfFreedomIds = this.DegreeOfFreedomIds;
        boundaryConditionIds = this.BoundaryConditionIds;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.DegreeOfFreedomIds;
        yield return this.BoundaryConditionIds;
    }
}
