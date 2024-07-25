using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

public class ModelSettings : BeamOSValueObject
{
    public UnitSettings UnitSettings { get; private set; }

    public bool YAxisUp { get; private set; }

    public ModelSettings(UnitSettings unitSettings, bool yAxisUp = false)
    {
        // required params
        this.UnitSettings = unitSettings;
        this.YAxisUp = yAxisUp;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.UnitSettings;
        yield return this.YAxisUp;
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ModelSettings() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
