using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

public abstract class NodeProposalBase : BeamOsModelProposalEntity<NodeProposalId, NodeId>
{
    protected NodeProposalBase(
        NodeProposalId id,
        ModelProposalId modelProposalId,
        ModelId modelId,
        NodeId? existingId = null
    )
        : base(id, modelProposalId, modelId, existingId) { }

    public abstract NodeDefinition ToDomain(
        Dictionary<Element1dProposalId, Element1d>? element1dProposalIdToNewIdDict = null
    );

    public abstract Point GetLocationPoint(
        IReadOnlyDictionary<Element1dId, Element1d> elementStore,
        IReadOnlyDictionary<NodeId, NodeDefinition> nodeStore
    );

    [Obsolete("EF Core Constructor")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected NodeProposalBase()
        : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public sealed class NodeProposal : NodeProposalBase
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

    public override Node ToDomain(
        Dictionary<Element1dProposalId, Element1d>? element1dProposalIdToNewIdDict = null
    )
    {
        return new(this.ModelId, this.LocationPoint, this.Restraint, this.ExistingId);
    }

    public override Point GetLocationPoint(
        IReadOnlyDictionary<Element1dId, Element1d> element1dStore,
        IReadOnlyDictionary<NodeId, NodeDefinition> nodeStore
    ) => this.LocationPoint;

    [Obsolete("EF Core Constructor")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private NodeProposal()
        : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public sealed class InternalNodeProposal : NodeProposalBase
{
    public InternalNodeProposal(
        ModelId modelId,
        ModelProposalId modelProposalId,
        Ratio ratioAlongElement1d,
        ExistingOrProposedElement1dId element1dId,
        Restraint restraint,
        NodeId? existingId = null,
        NodeProposalId? id = null
    )
        : base(id ?? new(), modelProposalId, modelId, existingId)
    {
        this.RatioAlongElement1d = ratioAlongElement1d;
        this.Element1dId = element1dId;
        this.Restraint = restraint;
    }

    public InternalNodeProposal(
        InternalNode existingNode,
        ModelProposalId modelProposalId,
        Ratio? ratioAlongElement1d,
        ExistingOrProposedElement1dId? element1dId,
        Restraint? restraint = null,
        NodeProposalId? id = null
    )
        : this(
            existingNode.ModelId,
            modelProposalId,
            ratioAlongElement1d ?? existingNode.RatioAlongElement1d,
            element1dId ?? new(existingId: existingNode.Element1dId.Id),
            restraint ?? existingNode.Restraint,
            existingNode.Id,
            id ?? new()
        ) { }

    public Restraint Restraint { get; set; }
    public Ratio RatioAlongElement1d { get; set; }
    public ExistingOrProposedElement1dId Element1dId { get; set; }

    // public Element1dProposal? Element1dProposal { get; set; }

    public override InternalNode ToDomain(
        Dictionary<Element1dProposalId, Element1d>? element1dProposalIdToNewIdDict = null
    )
    {
        var (element1dId, element1d) = this.Element1dId.ToIdAndEntity(
            element1dProposalIdToNewIdDict
        );
        return new InternalNode(
            this.ModelId,
            this.RatioAlongElement1d,
            element1dId,
            this.Restraint,
            this.ExistingId
        )
        {
            Element1d = element1d,
        };
    }

    public override Point GetLocationPoint(
        IReadOnlyDictionary<Element1dId, Element1d> element1dStore,
        IReadOnlyDictionary<NodeId, NodeDefinition> nodeStore
    )
    {
        Element1d el = this.Element1dId.Match(
            existingId =>
            {
                if (element1dStore.TryGetValue(existingId, out var element1d))
                {
                    return element1d;
                }

                throw new KeyNotFoundException(
                    $"Element1d with ID {existingId} not found in element1d store."
                );
            },
            static proposedId =>
                throw new NotImplementedException(
                    "Getting location for internal node proposal that is based on a proposed element1d is not implemented."
                )
        );

        return el.GetPointAtRatio(this.RatioAlongElement1d, element1dStore, nodeStore);
    }

    [Obsolete("EF Core Constructor")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private InternalNodeProposal()
        : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
