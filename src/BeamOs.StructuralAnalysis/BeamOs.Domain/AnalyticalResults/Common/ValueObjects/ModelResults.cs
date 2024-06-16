using BeamOs.Domain.AnalyticalResults.Element1dResultAggregate;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.Domain.Common.Models;

namespace BeamOs.Domain.AnalyticalResults.Common.ValueObjects;

public class ModelResults(List<NodeResult> nodeResults, List<Element1dResult> element1dResults)
    : BeamOSValueObject
{
    public List<NodeResult> NodeResults { get; } = nodeResults;
    public List<Element1dResult> Element1dResults { get; } = element1dResults;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach (var nodeResult in this.NodeResults)
        {
            yield return nodeResult;
        }

        foreach (var elResult in this.Element1dResults)
        {
            yield return elResult;
        }
    }
}
