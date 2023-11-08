using BeamOS.Common.Domain.Models;

namespace BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate.ValueObjects;
public class AnalyticalModelSettings : BeamOSValueObject
{
    public UnitSettings UnitSettings { get; }

    public AnalyticalModelSettings(UnitSettings unitSettings)
    {
        // required params
        this.UnitSettings = unitSettings;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.UnitSettings;
    }
}
