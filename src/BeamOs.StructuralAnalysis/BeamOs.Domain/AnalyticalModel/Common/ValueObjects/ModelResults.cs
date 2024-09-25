using BeamOs.Common.Domain.Models;
using BeamOs.Domain.AnalyticalModel.Element1dResultAggregate;
using BeamOs.Domain.AnalyticalModel.NodeResultAggregate;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using UnitsNet;

namespace BeamOs.Domain.AnalyticalModel.Common.ValueObjects;

public class ModelResults : BeamOSValueObject
{
    public ICollection<NodeResult>? NodeResults { get; init; }
    public List<Element1dResult>? Element1dResults { get; init; }
    public ShearForceDiagram[]? ShearForceDiagrams { get; init; }
    public MomentDiagram[]? MomentDiagrams { get; init; }
    public required Force MaxShearValue { get; init; }
    public required Force MinShearValue { get; init; }
    public required Torque MaxMomentValue { get; init; }
    public required Torque MinMomentValue { get; init; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        throw new NotImplementedException();
    }
}
