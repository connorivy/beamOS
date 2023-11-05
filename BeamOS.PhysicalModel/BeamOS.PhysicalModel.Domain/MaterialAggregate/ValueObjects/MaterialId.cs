using BeamOS.Common.Domain.Models;

namespace BeamOS.PhysicalModel.Domain.MaterialAggregate.ValueObjects;
public class MaterialId : BeamOSValueObject
{
    public Guid Value { get; }
    private MaterialId(Guid value)
    {
        this.Value = value;
    }
    public static MaterialId CreateUnique()
    {
        return new(Guid.NewGuid());
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Value;
    }
}
