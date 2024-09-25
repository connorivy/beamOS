using BeamOs.Common.Domain.Models;
using BeamOs.Domain.AnalyticalModel.AnalyticalResultsAggregate.ValueObjects;
using BeamOs.Domain.AnalyticalModel.NodeResultAggregate.ValueObjects;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;

namespace BeamOs.Domain.AnalyticalModel.NodeResultAggregate;

public class NodeResult : AggregateRoot<NodeResultId>
{
    public NodeResult(
        AnalyticalResultsId modelResultId,
        NodeId nodeId,
        Forces forces,
        Displacements displacements,
        NodeResultId? id = null
    )
        : base(id ?? new())
    {
        this.ModelResultId = modelResultId;
        this.NodeId = nodeId;
        this.Forces = forces;
        this.Displacements = displacements;
    }

    public AnalyticalResultsId ModelResultId { get; private set; }
    public NodeId NodeId { get; private set; }
    public Forces Forces { get; private set; }
    public Displacements Displacements { get; private set; }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private NodeResult() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
