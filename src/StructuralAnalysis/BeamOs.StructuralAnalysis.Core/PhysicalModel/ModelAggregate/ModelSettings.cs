using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

internal class ModelSettings : BeamOSValueObject
{
    public UnitSettings UnitSettings { get; private set; }
    public AnalysisSettings AnalysisSettings { get; private set; }
    public bool YAxisUp { get; private set; }

    public ModelSettings(
        UnitSettings unitSettings,
        bool yAxisUp,
        AnalysisSettings? analysisSettings = null
    )
    {
        this.UnitSettings = unitSettings;
        this.AnalysisSettings = analysisSettings ?? new();
        this.YAxisUp = yAxisUp;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.UnitSettings;
        yield return this.AnalysisSettings;
        yield return this.YAxisUp;
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ModelSettings() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
