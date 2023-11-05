using BeamOS.Common.Domain.Enums;
using BeamOS.Common.Domain.Models;

namespace BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate.ValueObjects;
public class AnalyticalNodeId : BeamOSValueObject
{
    public Guid Value { get; }
    private AnalyticalNodeId(Guid value)
    {
        this.Value = value;
    }
    public static AnalyticalNodeId CreateUnique()
    {
        return new(Guid.NewGuid());
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Value;
    }

    public IEnumerable<UnsupportedStructureDisplacementId> GetUnsupportedStructureDisplacementIds()
    {
        foreach (CoordinateSystemDirection3D direction in Enum.GetValues(typeof(CoordinateSystemDirection3D)))
        {
            if (direction == CoordinateSystemDirection3D.Undefined)
            {
                continue;
            }
            yield return new(this, direction);
        }
    }
}
