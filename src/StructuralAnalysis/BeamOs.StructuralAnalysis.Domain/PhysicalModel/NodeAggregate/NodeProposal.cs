using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

public sealed class NodeProposal : BeamOsModelProposalEntity<NodeProposalId, Node, NodeId>
{
    public NodeProposal(
        ModelId modelId,
        ModelProposalId modelProposalId,
        Point locationPoint,
        Restraint restraint,
        NodeId? existingId = null,
        NodeProposalId? id = null
    )
        : base(id ?? new(), modelProposalId, modelId, existingId)
    {
        this.LocationPoint = locationPoint;
        this.Restraint = restraint;
    }

    public NodeProposal(
        Node existingNode,
        ModelProposalId modelProposalId,
        Point? locationPoint = null,
        Restraint? restraint = null,
        NodeProposalId? id = null
    )
        : base(id ?? new(), modelProposalId, existingNode.ModelId, existingNode.Id)
    {
        this.LocationPoint = locationPoint ?? existingNode.LocationPoint;
        this.Restraint = restraint ?? existingNode.Restraint;
    }

    public Point LocationPoint { get; set; }
    public Restraint Restraint { get; set; }

    public Node ToDomain()
    {
        return new(this.ModelId, this.LocationPoint, this.Restraint, this.ExistingId);
    }

    [Obsolete("EF Core Constructor")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private NodeProposal()
        : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
