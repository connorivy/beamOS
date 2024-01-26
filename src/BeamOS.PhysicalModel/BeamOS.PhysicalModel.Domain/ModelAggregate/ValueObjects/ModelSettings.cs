using BeamOS.Common.Domain.Models;

namespace BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;

public class ModelSettings : BeamOSValueObject
{
    public UnitSettings UnitSettings { get; private set; }

    public ModelSettings(UnitSettings unitSettings)
    {
        // required params
        this.UnitSettings = unitSettings;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.UnitSettings;
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ModelSettings() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
