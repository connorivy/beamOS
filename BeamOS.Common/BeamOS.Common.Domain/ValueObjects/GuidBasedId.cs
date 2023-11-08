using BeamOS.Common.Domain.Models;

namespace BeamOS.Common.Domain.ValueObjects;
public abstract class GuidBasedId(Guid? value) : BeamOSValueObject
{
    public Guid Value { get; } = value ?? Guid.NewGuid();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Value;
    }
    public override string ToString()
    {
        return this.Value.ToString();
    }
}
