using BeamOS.Common.Domain.Models;

namespace BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
public class ModelSettings : BeamOSValueObject
{
    public UnitSettings UnitSettings { get; }

    public ModelSettings(UnitSettings unitSettings)
    {
        // required params
        this.UnitSettings = unitSettings;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.UnitSettings;
    }
}
