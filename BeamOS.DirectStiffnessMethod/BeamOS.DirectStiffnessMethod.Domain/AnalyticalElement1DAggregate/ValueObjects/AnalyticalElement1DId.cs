using BeamOS.Common.Domain.Models;

namespace BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate.ValueObjects;
public class AnalyticalElement1DId : BeamOSValueObject
{
    public Guid Value { get; }
    private AnalyticalElement1DId(Guid value)
    {
        this.Value = value;
    }
    public static AnalyticalElement1DId CreateUnique()
    {
        return new(Guid.NewGuid());
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Value;
    }
}
