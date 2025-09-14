using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;

internal sealed class EnvelopeResultSet : BeamOsModelEntity<EnvelopeResultSetId>
{
    public EnvelopeResultSet(ModelId modelId, EnvelopeResultSetId? id = null)
        : base(id ?? new(), modelId) { }

    private Dictionary<Element1dId, EnvelopeElement1dResult>? element1dResults;

    public IList<EnvelopeElement1dResult>? Element1dResults
    {
        get => this.element1dResults?.Values.ToList();
        private set => this.element1dResults = value.ToDictionary(k => k.Element1dId);
    }

    public void AddElement1dResult(Element1dResult element1dResult)
    {
        this.element1dResults ??= [];
        if (!this.element1dResults.TryGetValue(element1dResult.Id, out var existingResultSet))
        {
            this.element1dResults[element1dResult.Id] = this.Create(element1dResult);
        }
        else
        {
            existingResultSet.MergeInResult(element1dResult);
        }
    }

    public EnvelopeElement1dResult Create(Element1dResult element1DResult)
    {
        return new(
            element1DResult.ModelId,
            this.Id,
            element1DResult.Element1dId,
            new ResultSetAndQuantity<Force>(element1DResult.ResultSetId, element1DResult.MaxShear),
            new ResultSetAndQuantity<Force>(element1DResult.ResultSetId, element1DResult.MinShear),
            new ResultSetAndQuantity<Torque>(
                element1DResult.ResultSetId,
                element1DResult.MaxMoment
            ),
            new ResultSetAndQuantity<Torque>(
                element1DResult.ResultSetId,
                element1DResult.MinMoment
            ),
            new ResultSetAndQuantity<Length>(
                element1DResult.ResultSetId,
                element1DResult.MaxDisplacement
            ),
            new ResultSetAndQuantity<Length>(
                element1DResult.ResultSetId,
                element1DResult.MinDisplacement
            )
        );
    }

    [Obsolete("EF Core Constructor", true)]
    private EnvelopeResultSet() { }
}
