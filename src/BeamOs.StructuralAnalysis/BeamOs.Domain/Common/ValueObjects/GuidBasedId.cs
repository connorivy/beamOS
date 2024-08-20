using BeamOs.Common.Domain.Models;

namespace BeamOs.Domain.Common.ValueObjects;

public class GuidBasedId(Guid? id) : BeamOSValueObject
{
    public Guid Id { get; protected set; } = id ?? Guid.NewGuid();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Id;
    }

    public override string ToString()
    {
        return this.Id.ToString();
    }
}
