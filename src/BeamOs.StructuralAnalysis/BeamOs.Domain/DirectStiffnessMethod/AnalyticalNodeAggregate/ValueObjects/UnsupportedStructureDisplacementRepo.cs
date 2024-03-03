using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate;

namespace BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate.ValueObjects;

public class UnsupportedStructureDisplacementRepo : BeamOSValueObject
{
    public List<UnsupportedStructureDisplacementId> DegreeOfFreedomIds { get; } = [];
    public List<UnsupportedStructureDisplacementId> BoundaryConditionIds { get; } = [];

    public UnsupportedStructureDisplacementRepo(IEnumerable<AnalyticalNode> nodes)
    {
        this.InitializeIdentifierMaps(nodes);
    }

    private void InitializeIdentifierMaps(IEnumerable<AnalyticalNode> nodes)
    {
        foreach (var node in nodes)
        {
            foreach (
                CoordinateSystemDirection3D direction in Enum.GetValues(
                    typeof(CoordinateSystemDirection3D)
                )
            )
            {
                if (direction == CoordinateSystemDirection3D.Undefined)
                {
                    continue;
                }

                // if UnsupportedStructureDisplacement is degree of freedom
                if (node.Restraint.GetValueInDirection(direction) == true)
                {
                    this.DegreeOfFreedomIds.Add(new(node.Id, direction));
                }
                else
                {
                    this.BoundaryConditionIds.Add(new(node.Id, direction));
                }
            }
        }
    }

    protected override IEnumerable<object> GetEqualityComponents() =>
        throw new NotImplementedException();
}
