using System.ComponentModel.DataAnnotations.Schema;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

internal class AnalysisSettings(Element1dAnalysisType element1DAnalysisType) : BeamOSValueObject
{
    public AnalysisSettings()
        : this(Element1dAnalysisType.Timoshenko) { }

    public Element1dAnalysisType Element1DAnalysisType { get; set; } = element1DAnalysisType;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Element1DAnalysisType;
    }
}

internal class WorkflowSettings : BeamOSValueObject
{
    public required ModelingMode ModelingMode { get; set; }

    /// <summary>
    /// If the current model is a BIM First Source model, this property contains the IDs of all models
    /// that were created from this BIM First Source model.
    /// </summary>
    [NotMapped]
    public IList<ModelId>? BimFirstModelIds { get; set; }

    [Obsolete("This only exists as a 'backing field' of sorts for EF Core. Do not use")]
    public string? BimFirstModelIdsSerialized
    {
        get =>
            this.BimFirstModelIds is null
                ? null
                : string.Join(',', this.BimFirstModelIds.Select(id => id.Id.ToString()));
        private set
        {
            if (string.IsNullOrEmpty(value))
            {
                this.BimFirstModelIds = null;
            }
            else
            {
                this.BimFirstModelIds =
                [
                    .. value
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(idStr => new ModelId(Guid.Parse(idStr))),
                ];
            }
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.ModelingMode;
    }
}
