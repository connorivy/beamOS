using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;

public class NodeResult : BeamOsAnalyticalResultEntity<NodeId>
{
    public NodeResult(
        ModelId modelId,
        ResultSetId resultSetId,
        NodeId nodeId,
        Forces forces,
        Displacements displacements
    )
        : base(nodeId, resultSetId, modelId)
    {
        this.Forces = forces;
        this.Displacements = displacements;
    }

    public Forces Forces { get; private init; }
    public Displacements Displacements { get; private init; }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private NodeResult() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
