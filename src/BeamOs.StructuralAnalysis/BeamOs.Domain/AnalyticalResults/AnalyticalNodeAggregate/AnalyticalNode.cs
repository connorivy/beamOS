using BeamOs.Domain.AnalyticalResults.AnalyticalNodeAggregate.ValueObjects;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;

namespace BeamOs.Domain.AnalyticalResults.AnalyticalNodeAggregate;

public sealed class AnalyticalNode : AggregateRoot<AnalyticalNodeId>
{
    public AnalyticalNode(DsmNodeId nodeId, Forces forces, Displacements displacements)
    {
        this.NodeId = nodeId;
        this.Forces = forces;
        this.Displacements = displacements;
    }

    public DsmNodeId NodeId { get; private set; }
    public Forces Forces { get; private set; }
    public Displacements Displacements { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private AnalyticalNode() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
