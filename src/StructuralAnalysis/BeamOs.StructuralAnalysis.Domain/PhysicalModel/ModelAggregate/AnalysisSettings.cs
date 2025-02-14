using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

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
