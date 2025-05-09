using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;

public sealed class EnvelopeResultSet : BeamOsModelEntity<EnvelopeResultSetId>
{
    public EnvelopeResultSet(ModelId modelId, EnvelopeResultSetId? id = null)
        : base(id ?? new(), modelId) { }

    public Dictionary<Element1dId, EnvelopeElement1dResult> Element1dResults { get; set; } = [];

    public void AddElement1dResult(Element1dResult element1dResult)
    {
        if (!this.Element1dResults.TryGetValue(element1dResult.Id, out var existingResultSet))
        {
            this.Element1dResults[element1dResult.Id] = EnvelopeElement1dResult.Create(
                element1dResult
            );
        }
        else
        {
            existingResultSet.MergeInResult(element1dResult);
        }
    }

    [Obsolete("EF Core Constructor", true)]
    private EnvelopeResultSet() { }
}
