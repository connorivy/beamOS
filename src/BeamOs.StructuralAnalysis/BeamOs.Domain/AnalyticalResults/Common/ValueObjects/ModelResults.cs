using BeamOs.Domain.AnalyticalResults.Element1dResultAggregate;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;

namespace BeamOs.Domain.AnalyticalResults.Common.ValueObjects;

public class ModelResults : BeamOSValueObject
{
    public List<NodeResult>? NodeResults { get; init; }
    public List<Element1dResult>? Element1dResults { get; init; }
    public List<ShearForceDiagram>? ShearForceDiagrams { get; init; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        throw new NotImplementedException();
    }
}
