using BeamOS.Common.Domain.Models;

namespace BeamOS.PhysicalModel.Domain.SectionProfileAggregate.ValueObjects;
public class SectionProfileId : BeamOSValueObject
{
    public Guid Value { get; }
    private SectionProfileId(Guid value)
    {
        this.Value = value;
    }
    public static SectionProfileId CreateUnique()
    {
        return new(Guid.NewGuid());
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Value;
    }
}
