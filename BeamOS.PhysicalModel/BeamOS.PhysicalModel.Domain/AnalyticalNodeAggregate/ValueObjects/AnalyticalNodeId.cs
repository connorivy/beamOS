using BeamOS.Common.Domain.Models;

namespace BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate.ValueObjects;
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
}
