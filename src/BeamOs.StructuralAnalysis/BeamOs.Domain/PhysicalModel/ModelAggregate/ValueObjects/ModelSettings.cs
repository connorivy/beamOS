using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod;

namespace BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

public class ModelSettings : BeamOSValueObject
{
    public UnitSettings UnitSettings { get; private set; }
    public AnalysisSettings AnalysisSettings { get; private set; }

    public bool YAxisUp { get; private set; }

    public ModelSettings(UnitSettings unitSettings, AnalysisSettings? analysisSettings = null)
    {
        this.UnitSettings = unitSettings;
        this.AnalysisSettings = analysisSettings ?? new();

        // todo: support z-up
        this.YAxisUp = true;
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

public class AnalysisSettings(Element1dAnalysisType element1DAnalysisType) : BeamOSValueObject
{
    public AnalysisSettings()
        : this(Element1dAnalysisType.Timoshenko) { }

    public Element1dAnalysisType Element1DAnalysisType { get; set; } = element1DAnalysisType;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Element1DAnalysisType;
    }
}
