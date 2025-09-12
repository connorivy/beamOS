using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;

internal sealed class ResultSetAndQuantity<TQuantity>(ResultSetId resultSetId, TQuantity value)
    : BeamOSValueObject
    where TQuantity : IQuantity
{
    public ResultSetId ResultSetId { get; private set; } = resultSetId;
    public TQuantity Value { get; private set; } = value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.ResultSetId;
        yield return this.Value;
    }
}
