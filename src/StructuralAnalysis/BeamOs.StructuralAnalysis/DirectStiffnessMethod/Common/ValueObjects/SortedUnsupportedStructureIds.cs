using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.ValueObjects;

internal class SortedUnsupportedStructureIds(
    List<UnsupportedStructureDisplacementId> degreeOfFreedomIds,
    List<UnsupportedStructureDisplacementId> boundaryConditionIds
) : BeamOSValueObject
{
    public List<UnsupportedStructureDisplacementId> DegreeOfFreedomIds { get; } =
        degreeOfFreedomIds;
    public List<UnsupportedStructureDisplacementId> BoundaryConditionIds { get; } =
        boundaryConditionIds;

    public void Deconstruct(
        out List<UnsupportedStructureDisplacementId> degreeOfFreedomIds,
        out List<UnsupportedStructureDisplacementId> boundaryConditionIds
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
