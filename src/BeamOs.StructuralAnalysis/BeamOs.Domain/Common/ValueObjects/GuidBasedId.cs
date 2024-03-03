using BeamOs.Domain.Common.Models;

namespace BeamOs.Domain.Common.ValueObjects;

public class GuidBasedId(Guid? value) : BeamOSValueObject
{
    public Guid Value { get; protected set; } = value ?? Guid.NewGuid();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Value;
    }

    public override string ToString()
    {
        return this.Value.ToString();
    }
}
