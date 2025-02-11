using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.MomentDiagramAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.ShearForceDiagramAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;

public class ResultSet : BeamOsModelEntity<ResultSetId>
{
    public ResultSet(ModelId modelId, ResultSetId? id = null)
        : base(id ?? new(), modelId) { }

    public ICollection<NodeResult>? NodeResults { get; set; }
    public ICollection<Element1dResult>? Element1dResults { get; set; }

    //public ICollection<ShearForceDiagram>? ShearForceDiagrams { get; set; }
    //public ICollection<MomentDiagram>? MomentDiagrams { get; set; }

    [Obsolete("EF Core Constructor", true)]
    private ResultSet() { }
}
