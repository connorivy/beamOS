using BeamOs.Domain.Common.Models;

namespace BeamOs.Domain.Common.ValueObjects;

public class SingleValueWrapperValueObject<T> : BeamOSValueObject
{
    public T Value { get; }

    protected SingleValueWrapperValueObject(T value) => this.Value = value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Value;
    }

    public static implicit operator SingleValueWrapperValueObject<T>(T value) => new(value);

    public static implicit operator T(SingleValueWrapperValueObject<T> wrapper) => wrapper.Value;
}
