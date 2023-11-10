using BeamOS.Common.Domain.Models;

namespace BeamOS.DirectStiffnessMethod.Domain.ModelAggregate.ValueObjects;
public class ModelId : BeamOSValueObject
{
    public Guid Value { get; }
    private ModelId(Guid value)
    {
        this.Value = value;
    }
    public static ModelId CreateUnique()
    {
        return new(Guid.NewGuid());
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Value;
    }
}
