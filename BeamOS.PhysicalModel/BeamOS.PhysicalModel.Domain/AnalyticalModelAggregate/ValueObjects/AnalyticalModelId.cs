using BeamOS.Common.Domain.Models;

namespace BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate.ValueObjects;
public class AnalyticalModelId : BeamOSValueObject
{
    public Guid Value { get; }
    private AnalyticalModelId(Guid value)
    {
        this.Value = value;
    }
    public static AnalyticalModelId CreateUnique()
    {
        return new(Guid.NewGuid());
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Value;
    }
}
