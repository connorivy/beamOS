using BeamOS.Common.Domain.Models;

namespace BeamOS.DirectStiffnessMethod.Domain.Element1DAggregate.ValueObjects;
public class Element1DId : BeamOSValueObject
{
    public Guid Value { get; }
    private Element1DId(Guid value)
    {
        this.Value = value;
    }
    public static Element1DId CreateUnique()
    {
        return new(Guid.NewGuid());
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Value;
    }
}
