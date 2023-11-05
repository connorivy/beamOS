using BeamOS.Common.Domain.Models;

namespace BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;
public class PointLoadId : BeamOSValueObject
{
    public Guid Value { get; }
    private PointLoadId(Guid value)
    {
        this.Value = value;
    }
    public static PointLoadId CreateUnique()
    {
        return new(Guid.NewGuid());
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Value;
    }
}
